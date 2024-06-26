using System.Collections.Generic;
using System.Linq;
using OrchardCore.Rules.Models;

namespace OrchardCore.Rules.Services
{
    public class RuleMigrator : IRuleMigrator
    {
        private readonly IConditionIdGenerator _conditionIdGenerator;
        private readonly IEnumerable<IConditionFactory> _factories;

        public RuleMigrator(IConditionIdGenerator conditionIdGenerator, IEnumerable<IConditionFactory> factories)
        {
            _conditionIdGenerator = conditionIdGenerator;
            _factories = factories;
        }

        public void Migrate(string existingRule, Rule rule)
        {
            var factories = _factories.ToDictionary(x => x.Name);

            // Migrates well-known rules to well-known conditions.
            // A javascript condition is automatically created for less well-known rules. 
            switch (existingRule)
            {
                case "true":
                    if (factories.TryGetValue(nameof(BooleanCondition), out var booleanFactory))
                    {
                        var condition = (BooleanCondition)booleanFactory.Create();
                        _conditionIdGenerator.GenerateUniqueId(condition);
                        condition.Value = true;
                        rule.Conditions.Add(condition);
                    }
                    break;
                case "isHomepage()":
                    if (factories.TryGetValue(nameof(HomepageCondition), out var homepageFactory))
                    {
                        var condition = (HomepageCondition)homepageFactory.Create();
                        _conditionIdGenerator.GenerateUniqueId(condition);
                        condition.Value = true;
                        rule.Conditions.Add(condition);
                    }
                    break;
                case "isAnonymous()":
                    if (factories.TryGetValue(nameof(IsAnonymousCondition), out var isAnonymousFactory))
                    {
                        var condition = (IsAnonymousCondition)isAnonymousFactory.Create();
                        _conditionIdGenerator.GenerateUniqueId(condition);
                        rule.Conditions.Add(condition);
                    }
                    break;
                case "isAuthenticated()":
                    if (factories.TryGetValue(nameof(IsAuthenticatedCondition), out var isAuthenticatedFactory))
                    {
                        var condition = (IsAuthenticatedCondition)isAuthenticatedFactory.Create();
                        _conditionIdGenerator.GenerateUniqueId(condition);
                        rule.Conditions.Add(condition);
                    }
                    break;
                default:
                    if (factories.TryGetValue(nameof(JavascriptCondition), out var javascriptFactory))
                    {
                        var condition = (JavascriptCondition)javascriptFactory.Create();
                        _conditionIdGenerator.GenerateUniqueId(condition);
                        condition.Script = existingRule;
                        rule.Conditions.Add(condition);
                    }
                    break;
            }
        }
    }
}
