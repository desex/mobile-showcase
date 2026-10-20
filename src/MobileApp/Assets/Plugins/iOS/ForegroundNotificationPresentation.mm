#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <UserNotifications/UserNotifications.h>
#import "UnityAppController.h"
#import <string.h>

static NSString *const OpenActionKey = @"open_action";
static NSString *const NotificationUuidKey = @"notification_uuid";
static NSString *const MobilePushCategoryIdentifier = @"MOBILE_PUSH";
static NSString *pendingOpenAction = nil;
static NSString *pendingNotificationUuid = nil;
static NSString *pendingDismissOpenAction = nil;
static NSString *pendingDismissNotificationUuid = nil;

typedef void (*NotificationDismissCallback)(const char *openAction, const char *notificationUuid);
typedef void (*NotificationOpenCallback)(const char *openAction, const char *notificationUuid);
typedef void (*NotificationPresentedCallback)();
static NotificationOpenCallback notificationOpenCallback = NULL;
static NotificationDismissCallback notificationDismissCallback = NULL;
static NotificationPresentedCallback notificationPresentedCallback = NULL;

static NSString *StringValueFromUserInfo(NSDictionary *userInfo, NSString *key)
{
    id value = userInfo[key];

    if ([value isKindOfClass:[NSString class]]) {
        return value;
    }

    if ([value respondsToSelector:@selector(stringValue)]) {
        return [value stringValue];
    }

    return nil;
}

static NSString *StringValueFromCString(const char *value)
{
    if (value == NULL) {
        return @"null";
    }

    return [NSString stringWithUTF8String:value] ?: @"invalid_utf8";
}

static NSString *DelegateDescription(id<UNUserNotificationCenterDelegate> delegate)
{
    if (delegate == nil) {
        return @"nil";
    }

    return [NSString stringWithFormat:@"%@ <%p>", NSStringFromClass([delegate class]), delegate];
}

static void CachePendingNotificationOpen(NSDictionary *userInfo)
{
    NSString *openAction = StringValueFromUserInfo(userInfo, OpenActionKey);
    if (openAction.length == 0) {
        return;
    }

    pendingOpenAction = [openAction copy];
    pendingNotificationUuid = [StringValueFromUserInfo(userInfo, NotificationUuidKey) copy];
}

static BOOL NotifyNotificationOpen(NSDictionary *userInfo)
{
    if (notificationOpenCallback == NULL) {
        return NO;
    }

    NSString *openAction = StringValueFromUserInfo(userInfo, OpenActionKey);
    if (openAction.length == 0) {
        return NO;
    }

    NSString *notificationUuid = StringValueFromUserInfo(userInfo, NotificationUuidKey);
    notificationOpenCallback([openAction UTF8String], notificationUuid.length > 0 ? [notificationUuid UTF8String] : NULL);
    return YES;
}

static void CachePendingNotificationDismiss(NSDictionary *userInfo)
{
    NSString *openAction = StringValueFromUserInfo(userInfo, OpenActionKey);
    if (openAction.length == 0) {
        return;
    }

    pendingDismissOpenAction = [openAction copy];
    pendingDismissNotificationUuid = [StringValueFromUserInfo(userInfo, NotificationUuidKey) copy];
}

static BOOL NotifyNotificationDismiss(NSDictionary *userInfo)
{
    if (notificationDismissCallback == NULL) {
        return NO;
    }

    NSString *openAction = StringValueFromUserInfo(userInfo, OpenActionKey);
    if (openAction.length == 0) {
        return NO;
    }

    NSString *notificationUuid = StringValueFromUserInfo(userInfo, NotificationUuidKey);
    notificationDismissCallback([openAction UTF8String], notificationUuid.length > 0 ? [notificationUuid UTF8String] : NULL);
    return YES;
}

static void CopyNSStringToBuffer(NSString *value, char *buffer, int bufferSize)
{
    if (buffer == NULL || bufferSize <= 0) {
        return;
    }

    buffer[0] = '\0';

    if (value.length == 0) {
        return;
    }

    const char *utf8Value = [value UTF8String];
    if (utf8Value == NULL) {
        return;
    }

    strncpy(buffer, utf8Value, bufferSize - 1);
    buffer[bufferSize - 1] = '\0';
}

@interface ForegroundNotificationAppController : UnityAppController

@end

@implementation ForegroundNotificationAppController

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    NSDictionary *remoteNotification = launchOptions[UIApplicationLaunchOptionsRemoteNotificationKey];
    if ([remoteNotification isKindOfClass:[NSDictionary class]]) {
        NSLog(@"[ForegroundNotificationPresentation] didFinishLaunchingWithOptions remoteNotification: %@", remoteNotification);
        CachePendingNotificationOpen(remoteNotification);
    }

    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(ForegroundNotificationAppController)

@interface ForegroundNotificationPresentationDelegate : NSObject<UNUserNotificationCenterDelegate>

@property(nonatomic, weak) id<UNUserNotificationCenterDelegate> previousDelegate;

@end

@implementation ForegroundNotificationPresentationDelegate

- (void)userNotificationCenter:(UNUserNotificationCenter *)center
       willPresentNotification:(UNNotification *)notification
         withCompletionHandler:(void (^)(UNNotificationPresentationOptions options))completionHandler
{
    NSLog(@"[ForegroundNotificationPresentation] willPresentNotification userInfo: %@", notification.request.content.userInfo);

    if (notificationPresentedCallback != NULL) {
        notificationPresentedCallback();
    }

    if (@available(iOS 14.0, *)) {
        completionHandler(UNNotificationPresentationOptionBanner |
                          UNNotificationPresentationOptionList |
                          UNNotificationPresentationOptionSound |
                          UNNotificationPresentationOptionBadge);
    } else {
        completionHandler(UNNotificationPresentationOptionAlert |
                          UNNotificationPresentationOptionSound |
                          UNNotificationPresentationOptionBadge);
    }
}

- (void)userNotificationCenter:(UNUserNotificationCenter *)center
didReceiveNotificationResponse:(UNNotificationResponse *)response
         withCompletionHandler:(void (^)(void))completionHandler
{
    NSLog(@"[ForegroundNotificationPresentation] didReceiveNotificationResponse actionIdentifier: %@ userInfo: %@",
          response.actionIdentifier,
          response.notification.request.content.userInfo);

    if ([response.actionIdentifier isEqualToString:UNNotificationDismissActionIdentifier]) {
        if (!NotifyNotificationDismiss(response.notification.request.content.userInfo)) {
            CachePendingNotificationDismiss(response.notification.request.content.userInfo);
        }
    } else {
        CachePendingNotificationOpen(response.notification.request.content.userInfo);
        NotifyNotificationOpen(response.notification.request.content.userInfo);
    }

    id<UNUserNotificationCenterDelegate> previousDelegate = self.previousDelegate;

    if (previousDelegate != nil &&
        previousDelegate != self &&
        [previousDelegate respondsToSelector:@selector(userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:)]) {
        [previousDelegate userNotificationCenter:center
                  didReceiveNotificationResponse:response
                           withCompletionHandler:completionHandler];
        return;
    }

    completionHandler();
}

- (BOOL)respondsToSelector:(SEL)aSelector
{
    if ([super respondsToSelector:aSelector]) {
        return YES;
    }

    return [self.previousDelegate respondsToSelector:aSelector];
}

- (id)forwardingTargetForSelector:(SEL)aSelector
{
    if ([self.previousDelegate respondsToSelector:aSelector]) {
        return self.previousDelegate;
    }

    return [super forwardingTargetForSelector:aSelector];
}

@end

static ForegroundNotificationPresentationDelegate *foregroundNotificationPresentationDelegate = nil;

extern "C" void EnableForegroundNotificationPresentation()
{
    if (@available(iOS 10.0, *)) {
        UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];

        NSLog(@"[ForegroundNotificationPresentation] EnableForegroundNotificationPresentation currentDelegate: %@",
              DelegateDescription(center.delegate));

        UNNotificationCategory *category = [UNNotificationCategory categoryWithIdentifier:MobilePushCategoryIdentifier
                                                                                   actions:@[]
                                                                         intentIdentifiers:@[]
                                                                                   options:UNNotificationCategoryOptionCustomDismissAction];
        [center getNotificationCategoriesWithCompletionHandler:^(NSSet<UNNotificationCategory *> *existingCategories) {
            NSMutableSet<UNNotificationCategory *> *categories = [NSMutableSet set];
            if (existingCategories != nil) {
                [categories unionSet:existingCategories];
            }

            [categories addObject:category];
            [center setNotificationCategories:categories];
        }];

        if (foregroundNotificationPresentationDelegate == nil) {
            foregroundNotificationPresentationDelegate = [ForegroundNotificationPresentationDelegate new];
        }

        id<UNUserNotificationCenterDelegate> currentDelegate = center.delegate;
        if (currentDelegate != foregroundNotificationPresentationDelegate) {
            foregroundNotificationPresentationDelegate.previousDelegate = currentDelegate;
            center.delegate = foregroundNotificationPresentationDelegate;

            NSLog(@"[ForegroundNotificationPresentation] delegate installed previousDelegate: %@",
                  DelegateDescription(currentDelegate));
        }
    }
}

extern "C" void LogCurrentNotificationCenterDelegate(const char *source)
{
    if (@available(iOS 10.0, *)) {
        UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];

        NSLog(@"[ForegroundNotificationPresentation] delegate state source: %@, currentDelegate: %@, pluginDelegate: %@, previousDelegate: %@, isPluginCurrent: %@",
              StringValueFromCString(source),
              DelegateDescription(center.delegate),
              DelegateDescription(foregroundNotificationPresentationDelegate),
              foregroundNotificationPresentationDelegate == nil ? @"nil" : DelegateDescription(foregroundNotificationPresentationDelegate.previousDelegate),
              center.delegate == foregroundNotificationPresentationDelegate ? @"YES" : @"NO");
    }
}

extern "C" int TryConsumePendingNotificationOpen(char *openActionBuffer,
                                                 int openActionBufferSize,
                                                 char *notificationUuidBuffer,
                                                 int notificationUuidBufferSize)
{
    if (pendingOpenAction.length == 0) {
        return 0;
    }

    CopyNSStringToBuffer(pendingOpenAction, openActionBuffer, openActionBufferSize);
    CopyNSStringToBuffer(pendingNotificationUuid, notificationUuidBuffer, notificationUuidBufferSize);

    pendingOpenAction = nil;
    pendingNotificationUuid = nil;

    return 1;
}

extern "C" void ClearPendingNotificationOpen()
{
    pendingOpenAction = nil;
    pendingNotificationUuid = nil;
}

extern "C" int TryConsumePendingNotificationDismiss(char *openActionBuffer,
                                                    int openActionBufferSize,
                                                    char *notificationUuidBuffer,
                                                    int notificationUuidBufferSize)
{
    if (pendingDismissOpenAction.length == 0) {
        return 0;
    }

    CopyNSStringToBuffer(pendingDismissOpenAction, openActionBuffer, openActionBufferSize);
    CopyNSStringToBuffer(pendingDismissNotificationUuid, notificationUuidBuffer, notificationUuidBufferSize);

    pendingDismissOpenAction = nil;
    pendingDismissNotificationUuid = nil;

    return 1;
}

extern "C" void RegisterNotificationDismissCallback(NotificationDismissCallback callback)
{
    notificationDismissCallback = callback;
}

extern "C" void RegisterNotificationOpenCallback(NotificationOpenCallback callback)
{
    notificationOpenCallback = callback;
}

extern "C" void RegisterNotificationPresentedCallback(NotificationPresentedCallback callback)
{
    notificationPresentedCallback = callback;
}

extern "C" int GetNotificationAuthorizationStatus()
{
    if (@available(iOS 10.0, *)) {
        __block NSInteger authorizationStatus = UNAuthorizationStatusNotDetermined;
        dispatch_semaphore_t semaphore = dispatch_semaphore_create(0);

        [[UNUserNotificationCenter currentNotificationCenter]
            getNotificationSettingsWithCompletionHandler:^(UNNotificationSettings *settings) {
                authorizationStatus = settings.authorizationStatus;
                dispatch_semaphore_signal(semaphore);
            }];

        dispatch_semaphore_wait(semaphore, DISPATCH_TIME_FOREVER);
        return (int)authorizationStatus;
    }

    UIUserNotificationSettings *settings = [[UIApplication sharedApplication] currentUserNotificationSettings];
    return settings.types == UIUserNotificationTypeNone ? 1 : 2;
}