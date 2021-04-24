﻿using System;

namespace GraphProcessor
{
    public class PortTypeAttribute : Attribute
    {
        public Type PortType;
        public PortTypeAttribute(Type _portType) { PortType = _portType; }
    }
}