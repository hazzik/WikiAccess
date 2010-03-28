namespace WikiTools.Access
{
    /// <summary>
    /// Allows user to filter out pages from page list
    /// </summary>
    /// <param name="page">Page needed to be filtered</param>
    /// <returns>If true, page will be kept in page list. If false, page will be deleted.</returns>
    public delegate bool PageListFilter(Page page);
}