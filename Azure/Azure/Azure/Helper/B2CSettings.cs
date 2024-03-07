namespace Azure
{
    public static class B2CSettings
    {
        public static string TenantID = "2701e456-0937-4464-8dcf-2ecd91930a30";

        public static string ClientID = "54d7230e-1066-4351-a528-dc67302c5649";

        public static string[] Scopes = { "User.Read" };

        public static string IOSKeyChainGroup = "com.microsoft.adalcache";
        public static string BrokerRedirectUriOnIOS = "msauth.com.Fast.BusinessOnline://auth";
        public static string BrokerRedirectUriOnAndroid = "msauth://com.Fast.BusinessOnline/OFv01W4pmc7N2Evev6GFyXJnheg%3D";
    }
}
