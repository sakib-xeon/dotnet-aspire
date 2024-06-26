// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.ServiceDiscovery.Internal;

/// <summary>
/// Represents the result of service endpoint resolution.
/// </summary>
/// <param name="endpointSource">The endpoint collection.</param>
/// <param name="exception">The exception which occurred during resolution.</param>
internal sealed class ServiceEndpointResolverResult(ServiceEndpointSource? endpointSource, Exception? exception)
{
    /// <summary>
    /// Gets the exception which occurred during resolution.
    /// </summary>
    public Exception? Exception { get; } = exception;

    /// <summary>
    /// Gets a value indicating whether resolution completed successfully.
    /// </summary>
    [MemberNotNullWhen(true, nameof(EndpointSource))]
    public bool ResolvedSuccessfully => Exception is null;

    /// <summary>
    /// Gets the endpoints.
    /// </summary>
    public ServiceEndpointSource? EndpointSource { get; } = endpointSource;
}
