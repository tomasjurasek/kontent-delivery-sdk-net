﻿using Kentico.Kontent.Delivery.Abstractions;
using System;

namespace Kentico.Kontent.Delivery.StrongTyping
{
    internal class TypeProvider : ITypeProvider
    {
        public Type GetType(string contentType)
            => null;

        public string GetCodename(Type contentType)
            => null;
    }
}
