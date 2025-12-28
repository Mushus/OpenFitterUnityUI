import bpy
import os
import sys
import importlib

def check_status():
    print("[OpenFitter Status Check] Starting...")
    
    cwd = os.getcwd()
    scripts_path = os.path.join(cwd, "BlenderTools")
    addons_dir = os.path.join(scripts_path, "addons")
    addon_name = "robust-weight-transfer"
    
    # パス設定
    if scripts_path not in sys.path:
        sys.path.append(scripts_path)
    if addons_dir not in sys.path:
        sys.path.append(addons_dir)

    try:
        # アドオンを有効化して依存関係チェックを走らせる
        bpy.utils.refresh_script_paths()
        bpy.ops.preferences.addon_enable(module=addon_name)
        
        # アドオンモジュールを取得
        addon_module = importlib.import_module(addon_name)
        
        # アドオンが内部で保持している missing_deps を確認
        if hasattr(addon_module, "missing_deps") and not addon_module.missing_deps:
            print(f"RESULT:READY")
            # Unity側で検知しやすいようにマーカーファイルを作成
            marker_path = os.path.join(addons_dir, addon_name, "install_complete.marker")
            with open(marker_path, "w") as f:
                f.write("READY")
        else:
            missing = getattr(addon_module, "missing_deps", ["unknown"])
            print(f"RESULT:MISSING_DEPS:{missing}")
            
    except Exception as e:
        print(f"RESULT:ERROR:{str(e)}")

if __name__ == "__main__":
    check_status()
