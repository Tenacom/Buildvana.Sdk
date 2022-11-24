// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

// ---------------------------------------------------------------------------------------------
// Public API helpers
// ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;

using SysFile = System.IO.File;

/*
 * Summary : Specifies the kind of changes public APIs have undergone between an older and a newer version.
 * Remarks : The values of this enum are sorted in ascending order of importance,
 *           so that they may be compared.
 */
enum ApiChangeKind
{
    /*
     * Summary : Public APIs have not changed between two versions.
     */
    None,

    /*
     * Summary : A newer version has only added public APIs with respect to an older version.
     */
    Additive,

    /*
     * Summary : A newer version's public APIs have undergone breaking changes since an older version was published.
     */
    Breaking,
}

/*
 * Summary : Gets the kind of change public APIs underwent, according to the presence of new public APIs
 *           and/or the removal of existing public APIs in all PublicAPI.Unshipped.txt files
 *           of the repository.
 * Params  : context - The Cake context.
 * Returns : If at least one public API was removed, ApiChangeKind.Breaking;
 *           if no public API was removed, but at least one was added, ApiChangeKind.Additive;
 *           if no public API was removed nor added, ApiChangeKind.None.
 */
static ApiChangeKind GetPublicApiChangeKind(this ICakeContext context)
{
    context.Information("Computing API change kind according to unshipped public API files...");
    var result = ApiChangeKind.None;
    foreach (var unshippedPath in context.GetAllPublicApiFilePairs().Select(pair => pair.UnshippedPath))
    {
        var fileResult = context.GetPublicApiChangeKind(unshippedPath);
        context.Verbose($"{unshippedPath} -> {fileResult}");
        if (fileResult == ApiChangeKind.Breaking)
        {
            return ApiChangeKind.Breaking;
        }
        else if (fileResult > result)
        {
            result = fileResult;
        }
    }

    return result;
}

/*
 * Summary : Transfers unshipped public API definitions to PublicAPI.Shipped.txt
 *           in all directories of the repository.
 * Params  : context - The Cake context.
 * Returns : An enumeration of the modified files.
 */
static IEnumerable<FilePath> TransferAllPublicApiToShipped(this ICakeContext context)
{
    context.Information("Updating public API files...");
    foreach (var pair in context.GetAllPublicApiFilePairs())
    {
        context.Verbose($"Updating {pair.ShippedPath}...");
        if (context.TransferPublicApiToShipped(pair.UnshippedPath, pair.ShippedPath))
        {
            yield return pair.ShippedPath;
            yield return pair.UnshippedPath;
        }
    }
}

/*
 * Summary : Gets all public API definition file pairs in the repository.
 * Params  : context - The Cake context.
 * Returns : An enumeration of (UnshippedPath, ShippedPath) tuples.
 */
static IEnumerable<(FilePath UnshippedPath, FilePath ShippedPath)> GetAllPublicApiFilePairs(this ICakeContext context)
{
    (FilePath UnshippedPath, FilePath ShippedPath)? GetPair(FilePath shippedPath)
    {
        var unshippedPath = shippedPath.GetDirectory().CombineWithFilePath("PublicAPI.Unshipped.txt");
        return context.FileSystem.Exist(unshippedPath) ? (unshippedPath, shippedPath) : null;
    }

    return context
        .GetFiles("**/PublicAPI.Shipped.txt", new() { IsCaseSensitive = true })
        .Select(GetPair)
        .Where(maybePair => maybePair.HasValue)
        .Select(maybePair => maybePair!.Value);
}

/*
 * Summary : Gets the kind of change public APIs underwent, according to the presence of new public APIs
 *           and/or the removal of existing public APIs.
 * Params  : context       - The Cake context.
 *           unshippedPath - The FilePath of PublicAPI.Unshipped.txt
 * Returns : If at least one public API was removed, ApiChangeKind.Breaking;
 *           if no public API was removed, but at least one was added, ApiChangeKind.Additive;
 *           if no public API was removed nor added, ApiChangeKind.None.
 */
static ApiChangeKind GetPublicApiChangeKind(this ICakeContext context, FilePath unshippedPath)
{
    var unshippedLines = SysFile.ReadAllLines(unshippedPath.FullPath, Encoding.UTF8);
    static bool IsEmptyOrStartsWithHash(string s) => s.Length == 0 || s[0] == '#';
    var unshippedPublicApiLines = unshippedLines.SkipWhile(IsEmptyOrStartsWithHash);
    const string RemovedPrefix = "*REMOVED*";
    var newApiPresent = false;
    foreach (var line in unshippedPublicApiLines)
    {
        if (line.StartsWith(RemovedPrefix, StringComparison.Ordinal))
        {
            return ApiChangeKind.Breaking;
        }

        newApiPresent = true;
    }

    return newApiPresent ? ApiChangeKind.Additive : ApiChangeKind.None;
}

/*
 * Summary : Transfers unshipped public API definitions to PublicAPI.Shipped.txt
 * Params  : context       - The Cake context.
 *           unshippedPath - The FilePath of PublicAPI.Unshipped.txt
 *           shippedPath   - The FilePath of PublicAPI.Shipped.txt
 * Returns : true if files were modified; false otherwise.
 */
static bool TransferPublicApiToShipped(this ICakeContext context, FilePath unshippedPath, FilePath shippedPath)
{
    var utf8 = new UTF8Encoding(false);
    var unshippedLines = SysFile.ReadAllLines(unshippedPath.FullPath, utf8);
    var unshippedHeaderLines = unshippedLines.TakeWhile(IsEmptyOrStartsWithHash).ToArray();
    if (unshippedHeaderLines.Length == unshippedLines.Length)
    {
        return false;
    }

    static bool IsEmptyOrStartsWithHash(string s) => s.Length == 0 || s[0] == '#';
    var shippedLines = SysFile.ReadAllLines(shippedPath.FullPath, utf8);
    var shippedHeaderLines = shippedLines.TakeWhile(IsEmptyOrStartsWithHash).ToArray();

    const string RemovedPrefix = "*REMOVED*";
    static bool StartsWithRemovedPrefix(string s) => s.StartsWith(RemovedPrefix, StringComparison.Ordinal);
    static bool DoesNotStartWithRemovedPrefix(string s) => !StartsWithRemovedPrefix(s);
    var removedLines = unshippedLines
        .Skip(unshippedHeaderLines.Length)
        .Where(StartsWithRemovedPrefix)
        .Select(l => l[(RemovedPrefix.Length)..])
        .OrderBy(l => l, StringComparer.Ordinal) // For BinarySearch
        .ToArray();

    bool IsNotRemoved(string s) => Array.BinarySearch(removedLines, s, StringComparer.Ordinal) < 0;
    var newShippedLines = shippedLines
        .Skip(shippedHeaderLines.Length)
        .Where(IsNotRemoved)
        .Concat(unshippedLines
            .Skip(unshippedHeaderLines.Length)
            .Where(DoesNotStartWithRemovedPrefix))
        .OrderBy(l => l, StringComparer.Ordinal);

    SysFile.WriteAllLines(shippedPath.FullPath, shippedHeaderLines.Concat(newShippedLines), utf8);
    SysFile.WriteAllLines(unshippedPath.FullPath, unshippedHeaderLines, utf8);
    return true;
}
