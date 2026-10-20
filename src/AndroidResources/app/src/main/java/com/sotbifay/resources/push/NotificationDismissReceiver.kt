package com.sotbifay.resources.push

import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.util.Log

class NotificationDismissReceiver : BroadcastReceiver() {


    init {
        Log.d("NotificationDismiss", "Receiver created")
    }

    override fun onReceive(context: Context, intent: Intent?) {

        Log.d("NotificationDismiss", "Notification dismissed")

        val broadcast = Intent(ACTION_PUSH_DISMISSED).apply {
            setPackage(context.packageName)
        }

        context.sendBroadcast(broadcast)
    }
}