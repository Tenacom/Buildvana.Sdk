// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using JetBrains.Annotations;

namespace Buildvana.Sdk.Utilities
{
    /// <summary>
    /// Provides extension methods for <see cref="Exception"/> and derived classes.
    /// </summary>
    [PublicAPI]
    public static class ExceptionExtensions
    {
        // These all derive from IOException:
        //   * DirectoryNotFoundException
        //   * DriveNotFoundException
        //   * EndOfStreamException
        //   * FileLoadException
        //   * FileNotFoundException
        //   * PathTooLongException
        //   * PipeException
        public static bool IsIORelatedException(this Exception @this)
            => @this is UnauthorizedAccessException
            || @this is NotSupportedException
            || (@this is ArgumentException && @this is not ArgumentNullException)
            || @this is SecurityException
            || @this is IOException;

        /// <summary>
        /// Returns a value that tells whether an <see cref="Exception"/> is of a type that
        /// will likely cause application failure.
        /// </summary>
        /// <param name="this">The <see cref="Exception"/> on which this method is called.</param>
        /// <returns><see langword="true"/> if <paramref name="this"/> is a fatal exception;
        /// otherwise, <see langword="false"/>.</returns>
        /// <exception cref="NullReferenceException"><paramref name="this"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <para>This method recursively checks inner exceptions; and returns <see langword="true"/>
        /// if a fatal exception is found at any level of nesting.</para>
        /// <para>The following exception types are considered fatal exceptions:</para>
        /// <list type="bullet">
        /// <item><description><see cref="StackOverflowException"/></description></item>
        /// <item><description><see cref="OutOfMemoryException"/></description></item>
        /// <item><description><see cref="ThreadAbortException"/></description></item>
        /// <item><description><see cref="AccessViolationException"/></description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="IsCriticalException"/>
        public static bool IsFatalException(this Exception @this)
            => @this.IsFatalExceptionCore()
            || (@this.InnerException?.IsFatalException() ?? false)
            || (@this is AggregateException aggregateException && aggregateException.InnerExceptions.Any(IsFatalException));

        /// <summary>
        /// <para>Determines whether an <see cref="Exception"/> is a critical exception.</para>
        /// <para>Critical exceptions should normally not be caught just to be logged
        /// or ignored, as they are usually signs of a severe problem within a program.</para>
        /// </summary>
        /// <param name="this">The <see cref="Exception"/> on which this method is called.</param>
        /// <returns><see langword="true"/> if <paramref name="this"/> is a critical exception;
        /// otherwise, <see langword="false"/>.</returns>
        /// <exception cref="NullReferenceException"><paramref name="this"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <para>This method recursively checks inner exceptions; and returns <see langword="true"/>
        /// if a critical exception is found at any level of nesting.</para>
        /// <para>In addition to all fatal exceptions, as defined by the
        /// <see cref="IsFatalException"/> method, the following exception types are considered
        /// critical exceptions:</para>
        /// <list type="bullet">
        /// <item><description><see cref="AppDomainUnloadedException"/></description></item>
        /// <item><description><see cref="BadImageFormatException"/></description></item>
        /// <item><description><see cref="CannotUnloadAppDomainException"/></description></item>
        /// <item><description><see cref="InvalidProgramException"/></description></item>
        /// <item><description><see cref="NullReferenceException"/></description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="IsFatalException"/>
        public static bool IsCriticalException(this Exception @this)
            => @this.IsCriticalExceptionCore()
            || (@this.InnerException?.IsCriticalException() ?? false)
            || (@this is AggregateException aggregateException && aggregateException.InnerExceptions.Any(IsCriticalException));

        /// <summary>
        /// <para>Determines whether an <see cref="Exception"/> is either a security exception,
        /// or a critical exception.</para>
        /// </summary>
        /// <param name="this">The <see cref="Exception"/> on which this method is called.</param>
        /// <returns><see langword="true"/> if <paramref name="this"/> is either a <see cref="SecurityException"/>,
        /// or a critical exception as defined by the <see cref="IsCriticalException"/> method;
        /// otherwise, <see langword="false"/>.</returns>
        /// <exception cref="NullReferenceException"><paramref name="this"/> is <see langword="null"/>.</exception>
        /// <seealso cref="IsCriticalException"/>
        public static bool IsSecurityOrCriticalException(this Exception @this)
            => @this.IsSecurityOrCriticalExceptionCore()
            || (@this.InnerException?.IsSecurityOrCriticalException() ?? false)
            || (@this is AggregateException aggregateException && aggregateException.InnerExceptions.Any(IsSecurityOrCriticalException));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsFatalExceptionCore(this Exception @this)
            => @this is StackOverflowException
            || @this is OutOfMemoryException
            || @this is ThreadAbortException
            || @this is AccessViolationException;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsCriticalExceptionCore([System.Diagnostics.CodeAnalysis.NotNull] this Exception @this)
            => IsFatalExceptionCore(@this)
            || @this is AppDomainUnloadedException
            || @this is BadImageFormatException
            || @this is CannotUnloadAppDomainException
            || @this is InvalidProgramException
            || @this is NullReferenceException;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSecurityOrCriticalExceptionCore([System.Diagnostics.CodeAnalysis.NotNull] this Exception @this)
            => @this is SecurityException || IsCriticalExceptionCore(@this);
    }
}