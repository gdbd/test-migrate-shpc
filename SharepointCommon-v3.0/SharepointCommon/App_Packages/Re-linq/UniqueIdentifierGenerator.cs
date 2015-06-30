// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Linq
{
  /// <summary>
  /// Generates unique identifiers based on a set of known identifiers.
  /// An identifier is generated by appending a number to a given prefix. The identifier is considered unique when no known identifier
  /// exists which equals the prefix/number combination.
  /// </summary>
  public sealed class UniqueIdentifierGenerator
  {
    private readonly HashSet<string> _knownIdentifiers = new HashSet<string>();
    private int _identifierCounter;

    /// <summary>
    /// Adds the given <paramref name="identifier"/> to the set of known identifiers.
    /// </summary>
    /// <param name="identifier">The identifier to add.</param>
    public void AddKnownIdentifier (string identifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("identifier", identifier);
      _knownIdentifiers.Add (identifier);
    }

    private bool IsKnownIdentifier (string identifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("identifier", identifier);
      return _knownIdentifiers.Contains (identifier);
    }

    public void Reset ()
    {
      _knownIdentifiers.Clear();
      _identifierCounter = 0;
    }

    /// <summary>
    /// Gets a unique identifier starting with the given <paramref name="prefix"/>. The identifier is generating by appending a number to the
    /// prefix so that the resulting string does not match a known identifier.
    /// </summary>
    /// <param name="prefix">The prefix to use for the identifier.</param>
    /// <returns>A unique identifier starting with <paramref name="prefix"/>.</returns>
    public string GetUniqueIdentifier (string prefix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("prefix", prefix);

      string identifier;
      do
      {
        identifier = prefix + _identifierCounter;
        ++_identifierCounter;
      } while (IsKnownIdentifier (identifier));

      return identifier;
    }
  }
}