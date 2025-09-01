﻿using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.Role.Dto;

namespace ZhonTai.Admin.Services.Role;

/// <summary>
/// 角色接口
/// </summary>
public interface IRoleService
{
    Task<RoleGetOutput> GetAsync(long id);

    Task<List<RoleGetListOutput>> GetListAsync(RoleGetListInput input);

    Task<PageOutput<RoleGetPageOutput>> GetPageAsync(PageInput<RoleGetPageInput> input);

    Task<long> AddAsync(RoleAddInput input);

    Task AddRoleUserAsync(RoleAddRoleUserListInput input);

    Task RemoveRoleUserAsync(RoleAddRoleUserListInput input);

    Task UpdateAsync(RoleUpdateInput input);

    Task DeleteAsync(long id);

    Task BatchDeleteAsync(long[] ids);

    Task SoftDeleteAsync(long id);

    Task BatchSoftDeleteAsync(long[] ids);

    Task SetDataScopeAsync(RoleSetDataScopeInput input);
}