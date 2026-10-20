package com.sotbify.mobileapp.keyboardheight;

import android.app.Activity;
import android.graphics.Rect;
import android.os.Build;
import android.view.View;
import android.view.ViewTreeObserver;
import android.view.WindowInsets;

public final class KeyboardHeightPlugin {
    public interface Callback {
        void onKeyboardHeightChanged(int height);
    }

    private static Activity activity;
    private static View rootView;
    private static Callback callback;
    private static int cachedHeight;
    private static boolean registered;

    private static final ViewTreeObserver.OnGlobalLayoutListener globalLayoutListener = new ViewTreeObserver.OnGlobalLayoutListener() {
        @Override
        public void onGlobalLayout() {
            notifyIfChanged(calculateKeyboardHeight());
        }
    };

    private KeyboardHeightPlugin() {
    }

    public static void register(Activity unityActivity, Callback keyboardCallback) {
        if (registered) {
            callback = keyboardCallback;
            return;
        }

        activity = unityActivity;
        callback = keyboardCallback;

        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                rootView = activity.getWindow().getDecorView().getRootView();

                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
                    rootView.setOnApplyWindowInsetsListener(new View.OnApplyWindowInsetsListener() {
                        @Override
                        public WindowInsets onApplyWindowInsets(View view, WindowInsets insets) {
                            boolean isImeVisible = insets.isVisible(WindowInsets.Type.ime());
                            int height = isImeVisible ? insets.getInsets(WindowInsets.Type.ime()).bottom : 0;
                            notifyIfChanged(height);
                            return insets;
                        }
                    });
                }

                rootView.getViewTreeObserver().addOnGlobalLayoutListener(globalLayoutListener);
                rootView.requestApplyInsets();
                notifyIfChanged(calculateKeyboardHeight());
                registered = true;
            }
        });
    }

    public static void unregister() {
        if (!registered || activity == null) {
            callback = null;
            return;
        }

        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (rootView != null) {
                    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
                        rootView.setOnApplyWindowInsetsListener(null);
                    }

                    rootView.getViewTreeObserver().removeOnGlobalLayoutListener(globalLayoutListener);
                }

                rootView = null;
                activity = null;
                callback = null;
                cachedHeight = 0;
                registered = false;
            }
        });
    }

    public static int getHeight() {
        int height = calculateKeyboardHeight();
        cachedHeight = height;
        return height;
    }

    private static int calculateKeyboardHeight() {
        if (rootView == null) {
            return cachedHeight;
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            WindowInsets insets = rootView.getRootWindowInsets();
            if (insets != null) {
                boolean isImeVisible = insets.isVisible(WindowInsets.Type.ime());
                return isImeVisible ? insets.getInsets(WindowInsets.Type.ime()).bottom : 0;
            }
        }

        Rect visibleFrame = new Rect();
        rootView.getWindowVisibleDisplayFrame(visibleFrame);

        int rootHeight = rootView.getRootView().getHeight();
        int height = rootHeight - visibleFrame.bottom;
        return height > rootHeight * 0.15f ? height : 0;
    }

    private static void notifyIfChanged(int height) {
        if (cachedHeight == height) {
            return;
        }

        cachedHeight = height;

        if (callback != null) {
            callback.onKeyboardHeightChanged(height);
        }
    }
}