namespace DddTemplate.Domain.Abstractions;

/// <summary>
/// 领域服务标记接口
/// 领域服务用于封装不属于任何实体或值对象的领域逻辑
/// 通常用于：
/// 1. 跨多个聚合的业务逻辑
/// 2. 需要外部依赖的领域逻辑
/// 3. 复杂的业务规则计算
/// </summary>
public interface IDomainService
{
}
