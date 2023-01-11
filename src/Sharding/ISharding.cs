// -------------------------------------------------------------------------
// <copyright file="ISharding.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

namespace Sharding;

public interface ISharding<T> where T : class
{
    T GetNode(string key);
}