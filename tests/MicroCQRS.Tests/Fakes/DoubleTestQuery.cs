﻿namespace MicroCQRS.Tests.Fakes
{
    internal class DoubleTestQuery : IQuery<List<double>>
    {
        public double Prop { get; set; }
    }
}
