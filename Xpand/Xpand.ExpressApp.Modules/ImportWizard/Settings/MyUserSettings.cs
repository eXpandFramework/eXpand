﻿using System.Collections.Generic;
using System.Configuration;

namespace Xpand.ExpressApp.ImportWiz.Settings
{

    public class MyUserSettings : ApplicationSettingsBase
    {
        [UserScopedSetting]
        public List<string> MRUItems
        {
            get
            {
                return ((List<string>)this["MRUItems"]);
            }
            set
            {
                this["MRUItems"] = value;
            }
        }
       
    }

}