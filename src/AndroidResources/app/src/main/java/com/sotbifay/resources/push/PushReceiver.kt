package com.sotbifay.resources.push

import android.app.Activity
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.util.Log
import androidx.core.content.ContextCompat

class PushReceiver : BroadcastReceiver() {

    interface Listener {
        fun onPushReceived(
        )
    }

    companion object {

        const val ACTION_PUSH_RECEIVED = "com.sotbifay.mobileapp.push.RECEIVED"
        private var receiver: PushReceiver? = null
        private var listener: Listener? = null

        @JvmStatic
        fun register(activity: Activity, listener: Listener) {



            this.listener = listener

            if (receiver != null)
                return

            receiver = PushReceiver()

            ContextCompat.registerReceiver(
                activity,
                receiver,
                IntentFilter(ACTION_PUSH_RECEIVED),
                ContextCompat.RECEIVER_NOT_EXPORTED
            )
        }

        @JvmStatic
        fun unregister(activity: Activity) {

            receiver?.let {
                activity.unregisterReceiver(it)
            }

            receiver = null
            listener = null
        }
    }

    override fun onReceive(context: Context?, intent: Intent?) {
        Log.d("PushReceiver", "Broadcast received")
        listener?.onPushReceived(
        )
    }
}