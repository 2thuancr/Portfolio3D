using System;
using System.Threading.Tasks;
using Portfolio3D.Data;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Portfolio3D.Skills;

/* Seeds the real technologies actually used to build this project as skill
 * entries, grouped by category, for local development/testing. Skipped in
 * Production for the same reason as ProjectDataSeedContributor. Idempotent
 * by (Name, Category): never duplicates and never overwrites an
 * admin-edited skill. */
public class SkillDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private const string Backend = "Backend";
    private const string Frontend = "Frontend";
    private const string Database = "Database";
    private const string DevOpsTools = "DevOps/Tools";

    private readonly IRepository<Skill, Guid> _skillRepository;
    private readonly IGuidGenerator _guidGenerator;

    public SkillDataSeedContributor(
        IRepository<Skill, Guid> skillRepository,
        IGuidGenerator guidGenerator)
    {
        _skillRepository = skillRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task SeedAsync(DataSeedContext context)
    {
        if (SeedEnvironmentChecker.IsProductionEnvironment())
        {
            return;
        }

        /* DisplayOrder is assigned as one global sequence so that both the
         * item order within a category and the category group order
         * (Backend, Frontend, Database, DevOps/Tools) come out correct via
         * PublicPortfolioAppService's "sort then group" logic. */
        var displayOrder = 0;

        await SeedSkillAsync(".NET", Backend, displayOrder++);
        await SeedSkillAsync("ASP.NET Core", Backend, displayOrder++);
        await SeedSkillAsync("ABP Framework", Backend, displayOrder++);

        await SeedSkillAsync("Angular", Frontend, displayOrder++);
        await SeedSkillAsync("TypeScript", Frontend, displayOrder++);

        await SeedSkillAsync("PostgreSQL", Database, displayOrder++);
        await SeedSkillAsync("Redis", Database, displayOrder++);

        await SeedSkillAsync("Docker", DevOpsTools, displayOrder++);
        await SeedSkillAsync("Git", DevOpsTools, displayOrder);
    }

    private async Task SeedSkillAsync(string name, string category, int displayOrder)
    {
        var existing = await _skillRepository.GetListAsync(s => s.Name == name && s.Category == category);
        if (existing.Count > 0)
        {
            return;
        }

        var skill = new Skill(
            _guidGenerator.Create(),
            name,
            category,
            displayOrder: displayOrder
        );

        skill.Publish();

        await _skillRepository.InsertAsync(skill);
    }
}
