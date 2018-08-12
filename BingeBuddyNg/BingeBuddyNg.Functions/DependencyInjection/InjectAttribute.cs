using Microsoft.Azure.WebJobs.Description;
using System;

namespace BingeBuddyNg.Functions.DependencyInjection
{
    /// <summary>
    /// Support for DI in Azure Functions based on https://github.com/BorisWilhelms/azure-function-dependency-injection
    /// </summary>
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}
