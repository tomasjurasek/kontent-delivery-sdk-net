﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Kentico.Kontent.Delivery.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    class UseDisplayTemplateAttribute : UIHintAttribute
    {
        public UseDisplayTemplateAttribute(string uiHint)
            : base(uiHint)
        {
        }
    }
}
