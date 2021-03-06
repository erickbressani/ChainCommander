﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace ChainCommander.Sample.Implementation
{
    [ExcludeFromCodeCoverage]
    public class Human
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsEating { get; set; }
        public bool IsRunning { get; set; }
        public bool IsSleeping { get; set; }
        public bool IsWalking { get; set; }
        public bool IsWorking { get; set; }
    }
}
