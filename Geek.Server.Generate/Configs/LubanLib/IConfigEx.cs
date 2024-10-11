namespace Luban;

/// <summary>
/// 配置表数据扩展
/// </summary>
public interface IConfigEx
{
    /// <summary>
    /// 初始化，在加载完成配置表之后统一调用
    /// </summary>
    void onInit();
}