var Input4WebGL = {
	$input:null,
	$unityGameObjectName : "",
	$Inputing: function()
	{
		if(unityGameObjectName!=null && input != null)
		{
             var CaretPos = 0;
                        if (document.selection) {//支持IE
                            input.focus();
                            var Sel = document.selection.createRange();
                            Sel.moveStart('character', -input.value.length);
                            CaretPos = Sel.text.length;
                        }
                        else if (input.selectionStart || input.selectionStart == '0')//支持firefox
                            CaretPos = input.selectionStart;
			SendMessage(unityGameObjectName, "OnInputText",input.value);
			SendMessage(unityGameObjectName, "OnInputPos",CaretPos);
		}
	},
	$InputEnd: function()
	{
		if(unityGameObjectName!=null && input != null)
		{
			SendMessage(unityGameObjectName,"OnInputEnd");
		}
	},
	InputShow: function(GameObjectName_,v_,PosStart_,PosEnd_)
	{
		var GameObjectName = Pointer_stringify(GameObjectName_);
		var v = Pointer_stringify(v_);
		if(input==null){
			var inputdiv;
			input = document.createElement("input");
			inputdiv = document.createElement("div");
			inputdiv.style.position="absolute";
            inputdiv.style.top=0;
			input.type = "text";
			input.id = "Input4WebGlId";
			input.name = "Input4WebGl";
			input.style = "visibility:hidden;";
			input.oninput = Inputing;
			input.onblur = InputEnd;
			document.body.appendChild(inputdiv);
			inputdiv.appendChild(input);
		}
		input.value = v;
		unityGameObjectName = GameObjectName;
		input.style.visibility = "visible";  
		input.style.opacity = 0;
   		input.focus();
         if(input.setSelectionRange)
                    {
                        input.focus();
                        input.setSelectionRange(PosStart_,PosEnd_);
                    }
                    else if (input.createTextRange) {
                        var range = input.createTextRange();
                        range.collapse(true);
                        range.moveEnd('character',PosEnd_);
                        range.moveStart('character', PosStart_);
                        range.select();
                    }
	}
};
autoAddDeps(Input4WebGL, '$input');
autoAddDeps(Input4WebGL, '$Inputing');
autoAddDeps(Input4WebGL, '$InputEnd');
autoAddDeps(Input4WebGL, '$unityGameObjectName');
mergeInto(LibraryManager.library, Input4WebGL);