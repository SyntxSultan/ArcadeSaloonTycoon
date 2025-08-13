// MobileNotificationHelper.cs
using System;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public static class MobileNotificationHelper
{
    private const string ANDROID_CHANNEL_ID = "daily_rewards_channel";

    public static void Initialize()
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = ANDROID_CHANNEL_ID,
            Name = "Daily Rewards",
            Importance = Importance.Default,
            Description = "Notifications for daily rewards"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif

#if UNITY_IOS
        // Request permission on iOS
        var authOptions = AuthorizationOption.Alert | AuthorizationOption.Sound | AuthorizationOption.Badge;
        iOSNotificationCenter.RequestAuthorization(authOptions, true);
#endif
    }

    /// <summary>
    /// Schedule a notification to fire at the given DateTime (device local).
    /// Works cross-platform (Android/iOS). If fireTime is in the past, schedules in 5 seconds.
    /// Returns a string id (Android) or generated id for iOS (identifier).
    /// </summary>
    public static string ScheduleNotification(string title, string body, DateTime fireTime)
    {
        // ensure initialization called
        Initialize();

        // if fireTime already passed, push a short delay so user sees it shortly after close
        if (fireTime <= DateTime.Now) fireTime = DateTime.Now.AddSeconds(5);

#if UNITY_ANDROID
        var notif = new AndroidNotification()
        {
            Title = title,
            Text = body,
            FireTime = fireTime,
            SmallIcon = "default" // set in Project Settings -> Mobile Notifications or change accordingly
        };
        var id = AndroidNotificationCenter.SendNotification(notif, ANDROID_CHANNEL_ID);
        return id.ToString();
#elif UNITY_IOS
        var timeSpan = fireTime - DateTime.Now;
        if (timeSpan <= TimeSpan.Zero) timeSpan = TimeSpan.FromSeconds(5);

        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = timeSpan,
            Repeats = false
        };

        var notif = new iOSNotification()
        {
            Identifier = Guid.NewGuid().ToString(),
            Title = title,
            Body = body,
            ShowInForeground = true,
            ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
            CategoryIdentifier = "daily_rewards",
            ThreadIdentifier = "daily_rewards_thread",
            Trigger = timeTrigger
        };

        iOSNotificationCenter.ScheduleNotification(notif);
        return notif.Identifier;
#else
        Debug.Log("[MobileNotificationHelper] Platform doesn't support mobile notifications in editor.");
        return null;
#endif
    }

    public static void CancelAllNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
#endif
#if UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
    }
}
