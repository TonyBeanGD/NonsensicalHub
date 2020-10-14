local cuee = CS.UnityEngine;
local ui = CS.UnityEngine.UI;

-- 在 C# 的 Start 中调用
-- 用来初始化
function Awake(gObj)

    -- 获取一个 UnityEngine.UI 里的 Text
    local texts = GetComponentsInChildren(gObj, ui.Text);

    for i = 0, texts.Length - 1 do
        print(texts[i].text);
    end
end

-- 获取子组件
-- gObj 父组件
-- ty 要获取的子组件类型
-- tag 组件的tag
-- retys 返回要获取的所有组件
function GetComponentsInChildren(gObj, ty, tag)

    -- 获取所有 ty 类型的组件
    local tys = gObj:GetComponentsInChildren(typeof(ty));

    if tag == nil then

        -- 返回组件
        return tys;
    else
        -- 要返回的所有的 ty 类型的组件
        local retys = {};

        -- 循环查找 tag 相同的组件
        for i, v in pairs(tys) do

            -- 如果 tag 相等进入
            if v.gameObject.tag == tag then

                -- 将要获取的组件存到 retys 里
                table.insert(retys, v);
            end
        end

        -- 返回组件
        return retys;
    end

end

