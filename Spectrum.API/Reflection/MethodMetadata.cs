﻿using System;

namespace Spectrum.API.Reflection
{
    public class MethodMetadata
    {
        public bool IsStatic { get; set; }
        public string Name { get; set; }
        public Type[] Types { get; set; }
    }
}
