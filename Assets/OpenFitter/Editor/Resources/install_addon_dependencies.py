import bpy
import os
import sys

def main():
    # 1. アドオンパスの認識
    cwd = os.getcwd()
    scripts_path = os.path.join(cwd, "BlenderTools")
    
    # script_directories への追加（パスが通っていない場合）
    prefs = bpy.context.preferences
    found = False
    for path in prefs.filepaths.script_directories:
        if path.directory and os.path.abspath(path.directory) == os.path.abspath(scripts_path):
            found = True
            break
    
    if not found:
        print(f"[OpenFitter] Adding script directory: {scripts_path}")
        bpy.ops.preferences.script_directory_add(directory=scripts_path)

    # 2. アドオンの有効化
    addon_module_name = "robust-weight-transfer"
    try:
        print(f"[OpenFitter] Enabling addon: {addon_module_name}")
        bpy.utils.refresh_script_paths()
        bpy.ops.preferences.addon_enable(module=addon_module_name)
    except Exception as e:
        print(f"[OpenFitter] Error enabling addon: {e}")
        return

    # 3. アドオンのインストーラーを実行
    # robust-weight-transfer が提供する外部依存関係インストール機能を叩く
    if hasattr(bpy.ops.wm, "install_rwt_dependencies"):
        print("[OpenFitter] Triggering robust-weight-transfer dependency installer...")
        try:
            bpy.ops.wm.install_rwt_dependencies()
            print("[OpenFitter] Done.")
        except Exception as e:
            print(f"[OpenFitter] Error running internal installer: {e}")
            return
    else:
        print("[OpenFitter] Warning: 'wm.install_rwt_dependencies' operator not found.")
        return

    # 4. 設定を保存して永続化
    print("[OpenFitter] Saving Blender preferences...")
    bpy.ops.wm.save_userpref()
    print("[OpenFitter] Addon setup complete and preferences saved.")

if __name__ == "__main__":
    main()
