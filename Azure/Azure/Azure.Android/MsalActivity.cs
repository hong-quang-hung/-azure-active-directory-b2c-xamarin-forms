using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace Azure.Droid
{
    [Activity(Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth", DataScheme = "msal"
    )]
    public class MsalActivity : BrowserTabActivity
    {
    }
}