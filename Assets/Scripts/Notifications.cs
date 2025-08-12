using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.XR;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class Notifications : MonoBehaviour
{
    private const int NumberOfNotifications = 4;
    private static readonly int[] NotificationDelaysInHours = { 12, 24, 72 };
    private const string AndroidChannelId = "default_channel";

    void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        RegisterAndroidChannel();
        CancelAndroidNotifications();
        ScheduleAndroidNotifications();
#elif UNITY_IOS

//You must set the usage description in Info.plist manually or via Unity settings:,Go to Edit > Project Settings > iOS > Notification Settings or manually edit the Info.plist:,xml,Copy,Edit<key>NSUserNotificationUsageDescription</key> <string>This app uses notifications to remind you to recycle!</string>

        RequestIOSAuthorization();
        CancelIOSNotifications();
        ScheduleIOSNotifications();
#endif
    }

#if UNITY_ANDROID
    void RegisterAndroidChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = AndroidChannelId,
            Name = "Recycle Reminders",
            Importance = Importance.Default,
            Description = "Reminds the player to return and sort the waste.",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    void CancelAndroidNotifications()
    {
        AndroidNotificationCenter.CancelAllScheduledNotifications();
    }

    void ScheduleAndroidNotifications()
    {
        for (int i = 0; i < NumberOfNotifications; i++)
        {
            var hours = NotificationDelaysInHours[i];
            var notification = new AndroidNotification()
            {
                Title = "Time to Recycle!",
                Text = "Stressed or bored? Clear your head in 1 minute and beat your high score!",//casual igre se igraju kad ti je dosatno ili si stresiran, obecamo igracu da ce da resi to za 1 min, ako mu se igra dalje nastavice
                FireTime = System.DateTime.Now.AddHours(hours),
                //LargeIcon = "icon_96", // Uncomment when icon is ready
            };

            AndroidNotificationCenter.SendNotification(notification, AndroidChannelId);
        }
    }
#endif

#if UNITY_IOS
    void RequestIOSAuthorization()
    {
        iOSNotificationCenter.RequestAuthorization(
            AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound);
    }

    void CancelIOSNotifications()
    {
        iOSNotificationCenter.RemoveAllScheduledNotifications();
    }

    void ScheduleIOSNotifications()
    {
        for (int i = 0; i < NumberOfNotifications; i++)
        {
            var hours = NotificationDelaysInHours[i];
            var trigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new System.TimeSpan(hours, 0, 0),
                Repeats = false,
            };

            var notification = new iOSNotification()
            {
                Identifier = System.Guid.NewGuid().ToString(),
                Title = "Time to Recycle!",
                Body = "Stressed or bored? Clear your head in 1 minute and beat your high score!",
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                Trigger = trigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }
    }
#endif
}
