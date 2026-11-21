using Refit;

namespace EasyNetQ.Management.Client;

internal sealed class CriteriaQueryConverter :
    IQueryConverter<LengthsCriteria>,
    IQueryConverter<RatesCriteria>,
    IQueryConverter<PageCriteria>,
    IQueryConverter<StatsCriteria>,
    IQueryConverter<DeleteExchangeCriteria>,
    IQueryConverter<DeleteQueueCriteria>
{
    public void Flatten(LengthsCriteria value, string prefix, ref GeneratedQueryStringBuilder builder, RefitSettings settings) => Add(value.QueryParameters, ref builder);
    public void Flatten(RatesCriteria value, string prefix, ref GeneratedQueryStringBuilder builder, RefitSettings settings) => Add(value.QueryParameters, ref builder);
    public void Flatten(PageCriteria value, string prefix, ref GeneratedQueryStringBuilder builder, RefitSettings settings) => Add(value.QueryParameters, ref builder);
    public void Flatten(StatsCriteria value, string prefix, ref GeneratedQueryStringBuilder builder, RefitSettings settings) => Add(value.QueryParameters, ref builder);
    public void Flatten(DeleteExchangeCriteria value, string prefix, ref GeneratedQueryStringBuilder builder, RefitSettings settings) => Add(value.QueryParameters, ref builder);
    public void Flatten(DeleteQueueCriteria value, string prefix, ref GeneratedQueryStringBuilder builder, RefitSettings settings) => Add(value.QueryParameters, ref builder);

    private static void Add(IEnumerable<KeyValuePair<string, string>>? parameters, ref GeneratedQueryStringBuilder builder)
    {
        foreach (var parameter in parameters ?? [])
            builder.Add(parameter.Key, parameter.Value, false);
    }
}
