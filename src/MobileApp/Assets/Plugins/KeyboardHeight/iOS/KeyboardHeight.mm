//
//  KeyboardHeight.mm
//  Native plugin for getting keyboard height on iOS
//

#import <UIKit/UIKit.h>

static void (*_keyboardCallback)(int) = NULL;
static BOOL _keyboardPluginTerminating = NO;

@interface KeyboardHeightObserver : NSObject
@property (nonatomic, assign) CGFloat keyboardHeight;
@property (nonatomic, assign) BOOL keyboardVisible;
@property (nonatomic, strong) NSNotificationCenter *notificationCenter;
@end

@implementation KeyboardHeightObserver

- (instancetype)init {
    self = [super init];
    if (self) {
        _keyboardHeight = 0.0f;
        _keyboardVisible = NO;
        _notificationCenter = [NSNotificationCenter defaultCenter];
        
        [_notificationCenter addObserver:self
                           selector:@selector(keyboardWillShow:)
                               name:UIKeyboardWillShowNotification
                             object:nil];
        
        [_notificationCenter addObserver:self
                           selector:@selector(keyboardWillHide:)
                               name:UIKeyboardWillHideNotification
                             object:nil];

        [_notificationCenter addObserver:self
                           selector:@selector(applicationWillTerminate:)
                               name:UIApplicationWillTerminateNotification
                             object:nil];
    }
    return self;
}

- (void)dealloc {
    [_notificationCenter removeObserver:self];
}

- (void)keyboardWillShow:(NSNotification *)notification {
    if (_keyboardPluginTerminating) {
        return;
    }

    CGRect keyboardFrame = [notification.userInfo[UIKeyboardFrameEndUserInfoKey] CGRectValue];
    CGFloat keyboardHeight = keyboardFrame.size.height * [UIScreen mainScreen].scale;
    
    self.keyboardHeight = keyboardHeight;
    self.keyboardVisible = YES;
    
    [self notifyKeyboardHeightChanged:(int)_keyboardHeight];
}

- (void)keyboardWillHide:(NSNotification *)notification {
    if (_keyboardPluginTerminating) {
        return;
    }

    self.keyboardHeight = 0.0f;
    self.keyboardVisible = NO;
    
    [self notifyKeyboardHeightChanged:0];
}

- (void)applicationWillTerminate:(NSNotification *)notification {
    _keyboardPluginTerminating = YES;
    _keyboardCallback = NULL;
    self.keyboardHeight = 0.0f;
    self.keyboardVisible = NO;
    [_notificationCenter removeObserver:self];
}

- (void)notifyKeyboardHeightChanged:(int)height {
    if (_keyboardPluginTerminating || _keyboardCallback == NULL) {
        return;
    }

    dispatch_async(dispatch_get_main_queue(), ^{
        if (!_keyboardPluginTerminating && _keyboardCallback != NULL) {
            _keyboardCallback(height);
        }
    });
}

@end

static KeyboardHeightObserver *keyboardObserver = nil;

extern "C" {
    void RegisterKeyboardCallback(void (*callback)(int)) {
        if (_keyboardPluginTerminating) {
            return;
        }

        _keyboardCallback = callback;
        
        if (keyboardObserver == nil) {
            keyboardObserver = [[KeyboardHeightObserver alloc] init];
        }
    }

    void UnregisterKeyboardCallback() {
        _keyboardCallback = NULL;
    }

    bool IsKeyboardHeightPluginTerminating() {
        return _keyboardPluginTerminating;
    }
    
    int GetCachedKeyboardHeight() {
        if (_keyboardPluginTerminating) {
            return 0;
        }

        if (keyboardObserver == nil) {
            keyboardObserver = [[KeyboardHeightObserver alloc] init];
        }
        
        return (int)keyboardObserver.keyboardHeight;
    }
}