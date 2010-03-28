namespace WikiTools.Access
{
    /// <summary>
    /// Allows user to filter out pages from page list
    /// </summary>
    /// <typeparam name="T">Type of arameter for filter function</typeparam>
    /// <param name="page">Page needed to be filtered</param>
    /// <param name="param">Parameter for filter function</param>
    /// <returns>If true, page will be kept in page list. If false, page will be deleted.</returns>
    public delegate bool ParametrizedPageListFilter<T>(Page page, T param);
}