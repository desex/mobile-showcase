package com.sotbifay.resources.push

import android.annotation.SuppressLint
import android.Manifest
import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.PendingIntent
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.os.Build
import android.util.Log
import androidx.core.app.NotificationCompat
import androidx.core.app.NotificationManagerCompat
import androidx.core.app.Person
import androidx.core.content.ContextCompat
import androidx.core.graphics.drawable.IconCompat
import com.google.firebase.messaging.FirebaseMessagingService
import com.google.firebase.messaging.RemoteMessage
import com.sotbifay.resources.R
import kotlin.math.absoluteValue

const val ACTION_PUSH_RECEIVED = "com.sotbifay.mobileapp.push.RECEIVED"
const val ACTION_PUSH_DISMISSED = "com.sotbifay.mobileapp.push.DISMISSED"

class SotbifayFirebaseMessagingService : FirebaseMessagingService() {


    override fun onMessageReceived(message: RemoteMessage) {
        val data = message.data
        val intent = Intent(ACTION_PUSH_RECEIVED)
        intent.setPackage(packageName)
        Log.d(TAG, "onMessageReceived data=$data")

        val title = data[KEY_TITLE]
            ?: message.notification?.title
            ?: DEFAULT_NOTIFICATION_TITLE
        val body = data[KEY_BODY]
            ?: message.notification?.body
            ?: ""

        Log.d(TAG, "Sending ACTION_PUSH_RECEIVED broadcast")
        applicationContext.sendBroadcast(intent)

        showNotification(
            title = title,
            body = body,
            data = data
        )

    }

    private fun showNotification(
        title: String,
        body: String,
        data: Map<String, String>
    ) {
        Log.d(TAG, "showNotification title=$title body=$body open_action=${data[KEY_OPEN_ACTION]} uuid=${data[KEY_NOTIFICATION_UUID]}")
        createDefaultNotificationChannel()

        if (!hasNotificationPermission()) {
            Log.w(TAG, "showNotification skipped: POST_NOTIFICATIONS permission is not granted")
            return
        }

        val largeIcon = BitmapFactory.decodeResource(resources, R.drawable.icon_notification_large)
        val deleteIntent = Intent(this, NotificationDismissReceiver::class.java).apply {
            `package` = packageName
        }
        val notificationId = createNotificationId(data)
        val channelId = getString(R.string.sotbifay_default_notification_channel_id)
        val notificationPerson = createNotificationPerson(title, largeIcon)
        val notificationStyle = NotificationCompat.MessagingStyle(notificationPerson)
            .addMessage(body, System.currentTimeMillis(), notificationPerson)


        Log.d(TAG, "Creating delete PendingIntent")
        val deletePendingIntent = PendingIntent.getBroadcast(
            this,
            notificationId,
            deleteIntent,
            PendingIntent.FLAG_UPDATE_CURRENT or PendingIntent.FLAG_IMMUTABLE
        )

        Log.d(TAG, "deletePendingIntent=$deletePendingIntent")

        val notification = NotificationCompat.Builder(this, channelId)
            .setSmallIcon(R.drawable.icon_notification)
            .setContentTitle(title)
            .setContentText(body)
            .setStyle(notificationStyle)
            .setCategory(NotificationCompat.CATEGORY_MESSAGE)
            .setContentIntent(createContentIntent(data, notificationId))
            .setAutoCancel(true)
            .setPriority(NotificationCompat.PRIORITY_HIGH)
            .setDefaults(NotificationCompat.DEFAULT_ALL)
            .setDeleteIntent(deletePendingIntent)
            .build()

        notifySafely(notificationId, notification)
        Log.d(TAG, "Notification posted")
    }

    private fun createNotificationPerson(title: String, largeIcon: Bitmap?): Person {
        val builder = Person.Builder()
            .setName(title)

        if (largeIcon != null) {
            builder.setIcon(IconCompat.createWithBitmap(largeIcon))
        }

        return builder.build()
    }

    private fun hasNotificationPermission(): Boolean {
        val isGranted = Build.VERSION.SDK_INT < Build.VERSION_CODES.TIRAMISU ||
            ContextCompat.checkSelfPermission(this, Manifest.permission.POST_NOTIFICATIONS) == PackageManager.PERMISSION_GRANTED
        return isGranted
    }

    @SuppressLint("MissingPermission")
    private fun notifySafely(notificationId: Int, notification: android.app.Notification) {
        try {
            NotificationManagerCompat.from(this)
                .notify(notificationId, notification)
            Log.d(TAG, "notifySafely success notificationId=$notificationId")
        } catch (_: Exception) {
            Log.w(TAG, "notifySafely failed notificationId=$notificationId")
        }
    }

    private fun createContentIntent(data: Map<String, String>, notificationId: Int): PendingIntent? {
        val openIntent = Intent()
            .setClassName(packageName, ACTIVITY_CLASS_NAME)
            .addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_SINGLE_TOP)

        openIntent
            .putExtra(KEY_OPEN_ACTION, data[KEY_OPEN_ACTION])
            .putExtra(KEY_NOTIFICATION_UUID, data[KEY_NOTIFICATION_UUID])

        data.forEach { (key, value) ->
            openIntent.putExtra(key, value)
        }

        Log.d(
            TAG,
            "createContentIntent target=$ACTIVITY_CLASS_NAME open_action=${data[KEY_OPEN_ACTION]} uuid=${data[KEY_NOTIFICATION_UUID]}"
        )

        return PendingIntent.getActivity(
            this,
            notificationId,
            openIntent,
            PendingIntent.FLAG_UPDATE_CURRENT or PendingIntent.FLAG_IMMUTABLE
        )
    }

    private fun createDefaultNotificationChannel() {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.O) {
            return
        }

        val channelId = getString(R.string.sotbifay_default_notification_channel_id)
        val channelName = getString(R.string.sotbifay_default_notification_channel_name)
        val channel = NotificationChannel(
            channelId,
            channelName,
            NotificationManager.IMPORTANCE_HIGH
        )

        val notificationManager = getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager
        notificationManager.createNotificationChannel(channel)
    }

    private fun createNotificationId(data: Map<String, String>): Int {
        val uuid = data[KEY_NOTIFICATION_UUID]
        if (uuid.isNullOrBlank()) {
            return System.currentTimeMillis().toInt()
        }

        return uuid.hashCode().absoluteValue
    }

    companion object {
        private const val TAG = "SotbifayPushService"
        private const val DEFAULT_NOTIFICATION_TITLE = "Sotbifay"
        private const val KEY_TITLE = "title"
        private const val KEY_BODY = "body"
        private const val KEY_OPEN_ACTION = "open_action"
        private const val KEY_NOTIFICATION_UUID = "notification_uuid"
        private const val ACTIVITY_CLASS_NAME = "com.sotbifay.mobileapp.push.SotbifayMessagingUnityPlayerActivity"
    }
}