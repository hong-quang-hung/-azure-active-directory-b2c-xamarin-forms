namespace Azure
{
    public static class B2CSettings
    {
        public static string TenantID = "2701e456-0937-4464-8dcf-2ecd91930a30";
        public static string TenantName = "quynhnvhotmail";
        public static string Tenant = "quynhnvhotmail.onmicrosoft.com";
        public static string AzureADB2CHostname = "quynhnvhotmail.b2clogin.com";
        public static string ClientID = "54d7230e-1066-4351-a528-dc67302c5649";

        public static string PolicySignUpSignIn = "b2c_1_signupsignin";
        public static string PolicyEditProfile = "b2c_1_edit_profile";
        public static string PolicyResetPassword = "b2c_1_reset";

        public static string[] Scopes = { "https://quynhnvhotmail.onmicrosoft.com/helloapi/demo.read" };

        public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";

        public static string IOSKeyChainGroup = "com.microsoft.adalcache";
    }
}
