using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace Azure.Droid
{
    [Activity(Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth", DataScheme = "msal2701e456-0937-4464-8dcf-2ecd91930a30"
    )]
    public class MsalActivity : BrowserTabActivity
    {
    }
}