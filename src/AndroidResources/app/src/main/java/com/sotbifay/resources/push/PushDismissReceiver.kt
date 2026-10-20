package com.sotbifay.resources.push

import android.app.Activity
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import androidx.core.content.ContextCompat

class PushDismissReceiver : BroadcastReceiver() {

    interface Listener {
        fun onPushDismissed()
    }

    companion object {

        private var receiver: PushDismissReceiver? = null
        private var listener: Listener? = null

        @JvmStatic
        fun register(activity: Activity, listener: Listener) {

            this.listener = listener

            if (receiver != null)
                return

            receiver = PushDismissReceiver()

            ContextCompat.registerReceiver(
                activity,
                receiver,
                IntentFilter(ACTION_PUSH_DISMISSED),
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

        listener?.onPushDismissed()
    }
}