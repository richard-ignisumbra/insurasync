using System;
namespace permaAPI.services
{
    public class B2CService
    {
        public B2CService()
        { }


        private string _b2cExtensionAppClientId;

        private string B2cCustomAttributeHelper(string b2cExtensionAppClientId)
        {
            _b2cExtensionAppClientId = b2cExtensionAppClientId.Replace("-", "");
            return _b2cExtensionAppClientId;
        }



    }
}

