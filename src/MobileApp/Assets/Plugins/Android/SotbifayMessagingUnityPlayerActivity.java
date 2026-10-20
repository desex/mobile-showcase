package com.sotbifay.mobileapp.push;

import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;

public class SotbifayMessagingUnityPlayerActivity
    extends com.google.firebase.MessagingUnityPlayerActivity {

    private static final String TAG = "SotbifayPushActivity";
    private static final String KEY_OPEN_ACTION = "open_action";
    private static final String KEY_NOTIFICATION_UUID = "notification_uuid";
    private static final Object LOCK = new Object();

    private static String pendingOpenAction;
    private static String pendingNotificationUuid;
    private static String consumedNotificationUuid;
    private static NotificationOpenListener notificationOpenListener;

    public interface NotificationOpenListener {
        void onNotificationOpen(String openAction, String notificationUuid);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        handleNotificationOpenIntent(getIntent(), "onCreate");
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        handleNotificationOpenIntent(intent, "onNewIntent");
    }

    public static void registerListener(NotificationOpenListener listener) {
        synchronized (LOCK) {
            notificationOpenListener = listener;
        }

        Log.d(TAG, "registerListener: " + listener);
    }

    public static void unregisterListener() {
        synchronized (LOCK) {
            notificationOpenListener = null;
        }

        Log.d(TAG, "unregisterListener");
    }

    public static String consumePendingOpenAction() {
        synchronized (LOCK) {
            String openAction = pendingOpenAction;
            consumedNotificationUuid = pendingNotificationUuid;
            pendingOpenAction = null;
            pendingNotificationUuid = null;
            Log.d(TAG, "consumePendingOpenAction: " + openAction);
            return openAction;
        }
    }

    public static String consumePendingNotificationUuid() {
        synchronized (LOCK) {
            String notificationUuid = consumedNotificationUuid;
            consumedNotificationUuid = null;
            Log.d(TAG, "consumePendingNotificationUuid: " + notificationUuid);
            return notificationUuid;
        }
    }

    public static void clearPendingNotificationOpen() {
        synchronized (LOCK) {
            pendingOpenAction = null;
            pendingNotificationUuid = null;
            consumedNotificationUuid = null;
        }

        Log.d(TAG, "clearPendingNotificationOpen");
    }

    private void handleNotificationOpenIntent(Intent intent, String source) {
        if (intent == null) {
            Log.d(TAG, source + " skipped: intent is null");
            return;
        }

        String openAction = intent.getStringExtra(KEY_OPEN_ACTION);
        String notificationUuid = intent.getStringExtra(KEY_NOTIFICATION_UUID);

        Log.d(TAG, source + " open_action=" + openAction + ", notification_uuid=" + notificationUuid);

        if (TextUtils.isEmpty(openAction)) {
            return;
        }

        NotificationOpenListener listener;
        synchronized (LOCK) {
            listener = notificationOpenListener;
        }

        if (notifyListener(listener, openAction, notificationUuid, source)) {
            return;
        }

        cachePendingNotificationOpen(openAction, notificationUuid);
    }

    private static void cachePendingNotificationOpen(String openAction, String notificationUuid) {
        synchronized (LOCK) {
            pendingOpenAction = openAction;
            pendingNotificationUuid = notificationUuid;
            consumedNotificationUuid = null;
        }

        Log.d(TAG, "cachePendingNotificationOpen open_action=" + openAction + ", notification_uuid=" + notificationUuid);
    }

    private static boolean notifyListener(
        NotificationOpenListener listener,
        String openAction,
        String notificationUuid,
        String source
    ) {
        if (listener == null) {
            Log.d(TAG, source + " notify skipped: listener is null");
            return false;
        }

        try {
            listener.onNotificationOpen(openAction, notificationUuid);
            Log.d(TAG, source + " notify success");
            return true;
        } catch (Throwable throwable) {
            Log.w(TAG, source + " notify failed", throwable);
            return false;
        }
    }

}