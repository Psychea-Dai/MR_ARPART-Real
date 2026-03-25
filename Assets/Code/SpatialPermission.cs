using UnityEngine;

// 启动时请求访问房间空间数据的权限
public class SpatialPermission : MonoBehaviour
{
    // Start()在物体被加载后自动执行一次
    void Start()
    {
        string permission = "com.oculus.permission.USE_SCENE";

        // 检查用户是否已经授权过这个权限
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission))
        {
            // 没有授权，弹出系统对话框请求
            UnityEngine.Android.Permission.RequestUserPermission(permission);
        }
    }
}