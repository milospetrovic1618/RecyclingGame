using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class Notifications : MonoBehaviour
{
    private const int NumberOfNotifications = 4;
    private static readonly int[] NotificationDelaysInHours = { 1, 12, 24, 72 };
    private const string AndroidChannelId = "default_channel";

    void Start()
    {
#if UNITY_ANDROID
        RegisterAndroidChannel();
        CancelAndroidNotifications();
        ScheduleAndroidNotifications();
#elif UNITY_IOS
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
                Text = $"It's been {hours} hours. Sort the trash and clean the planet.",
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
                Body = $"It's been {hours} hours. Sort the trash and clean the planet.",
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                Trigger = trigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }
    }
#endif
}
