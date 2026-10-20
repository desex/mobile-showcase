#import <UIKit/UIKit.h>

extern "C" UIView* UnityGetGLView(void);

// The last reported keyboard frame in screen coordinates, CGRectNull while the keyboard is hidden.
static CGRect _keyboardFrame = CGRectNull;

@interface SoftwareKeyboardAreaObserver : NSObject
@end

@implementation SoftwareKeyboardAreaObserver

+ (void)load
{
    // Subscribe before Unity starts, so a keyboard opened at any moment is not missed.
    NSNotificationCenter* center = [NSNotificationCenter defaultCenter];

    [center addObserverForName: UIKeyboardWillChangeFrameNotification object: nil queue: nil usingBlock: ^(NSNotification* notification)
    {
        // A keyboard of another app in Split View / Slide Over does not belong to our input.
        NSNumber* isLocal = notification.userInfo[UIKeyboardIsLocalUserInfoKey];
        if (isLocal != nil && isLocal.boolValue == NO)
        {
            _keyboardFrame = CGRectNull;
            return;
        }

        NSValue* frame = notification.userInfo[UIKeyboardFrameEndUserInfoKey];
        _keyboardFrame = frame != nil ? frame.CGRectValue : CGRectNull;
    }];

    [center addObserverForName: UIKeyboardWillHideNotification object: nil queue: nil usingBlock: ^(NSNotification* notification)
    {
        _keyboardFrame = CGRectNull;
    }];
}

@end

// Returns the part of the Unity view height occluded by the keyboard, in [0, 1].
extern "C" float SoftwareKeyboardArea_GetNormalizedHeight()
{
    if (CGRectIsNull(_keyboardFrame) || CGRectIsEmpty(_keyboardFrame))
        return 0;

    UIView* view = UnityGetGLView();
    UIWindow* window = view.window;
    if (view == nil || window == nil)
        return 0;

    CGRect bounds = view.bounds;
    if (bounds.size.height <= 0)
        return 0;

    CGRect frame = [view convertRect: _keyboardFrame fromCoordinateSpace: window.screen.coordinateSpace];
    CGRect occluded = CGRectIntersection(bounds, frame);
    if (CGRectIsNull(occluded) || CGRectIsEmpty(occluded))
        return 0;

    // Floating and split keyboards on iPad are not attached to the bottom edge and do not cover the content.
    if (CGRectGetMaxY(bounds) - CGRectGetMaxY(occluded) > 1)
        return 0;

    return (float)((CGRectGetMaxY(bounds) - CGRectGetMinY(occluded)) / bounds.size.height);
}
