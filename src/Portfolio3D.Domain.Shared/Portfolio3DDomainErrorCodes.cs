namespace Portfolio3D;

public static class Portfolio3DDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */

    public const string ProjectSlugAlreadyExists = "Portfolio3D:Projects:SlugAlreadyExists";
    public const string ProjectReorderItemsEmpty = "Portfolio3D:Projects:ReorderItemsEmpty";
    public const string ProjectReorderDuplicateId = "Portfolio3D:Projects:ReorderDuplicateId";
    public const string ProjectReorderDuplicateDisplayOrder = "Portfolio3D:Projects:ReorderDuplicateDisplayOrder";
    public const string ProjectReorderProjectNotFound = "Portfolio3D:Projects:ReorderProjectNotFound";

    public const string SkillReorderItemsEmpty = "Portfolio3D:Skills:ReorderItemsEmpty";
    public const string SkillReorderDuplicateId = "Portfolio3D:Skills:ReorderDuplicateId";
    public const string SkillReorderDuplicateDisplayOrder = "Portfolio3D:Skills:ReorderDuplicateDisplayOrder";
    public const string SkillReorderSkillNotFound = "Portfolio3D:Skills:ReorderSkillNotFound";
}
