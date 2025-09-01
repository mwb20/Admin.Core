﻿using ZhonTai.Admin.Core.Entities;
using FreeSql.DataAnnotations;
using ZhonTai.Admin.Domain.Permission;
using ZhonTai.Admin.Domain.Pkg;

namespace ZhonTai.Admin.Domain.PkgPermission;

/// <summary>
/// 套餐权限
/// </summary>
[Table(Name = DbConsts.TableNamePrefix + "pkg_permission", OldName = DbConsts.TableOldNamePrefix + "pkg_permission")]
[Index("idx_{tablename}_01", nameof(Platform) + "," + nameof(PkgId) + "," + nameof(PermissionId), true)]
public class PkgPermissionEntity : EntityAdd
{
    /// <summary>
    /// 平台
    /// </summary>
    public string Platform { get; set; }

    /// <summary>
    /// 套餐Id
    /// </summary>
	[Column(IsPrimary = true)]
    public long PkgId { get; set; }

    /// <summary>
    /// 套餐
    /// </summary>
    public PkgEntity Pkg { get; set; }

    /// <summary>
    /// 权限Id
    /// </summary>
	[Column(IsPrimary = true)]
    public long PermissionId { get; set; }

    /// <summary>
    /// 权限
    /// </summary>
    public PermissionEntity Permission { get; set; }
}