using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Net.Sockets;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace CM3D2.RemoteControl.Plugin
{
	#region PluginMain
	[	PluginFilter( "CM3D2x64" ),
		PluginFilter( "CM3D2x86" ),
		PluginFilter( "CM3D2VRx64" ),
		PluginName( "CM3D2.RemoteControl.Plugin" ),
		PluginVersion( "0.0.1.0" )]
	public class RemoteControl : UnityInjector.PluginBase
	{
		#region Methods
        private TcpListener server;  // リスナー(接続待ちや受信等を行なうｵﾌﾞｼﾞｪｸﾄ)
        private System.Threading.Thread ListeningCallbackThread;   // 接続待ちスレッド

		private static int m_TcpServerPort = 9000;
/********************
	俺スレッド
******************/
	private volatile int my_thread_flag = 1;
    public void MyThreadProc()
    {
		Console.WriteLine( "{0}: Start TCP server on Port {1}", this.GetPluginName(), m_TcpServerPort );
		server = new TcpListener( System.Net.IPAddress.Any, m_TcpServerPort );
server.Start();
	// 受信の受付を行なうための無限ループ
	while( my_thread_flag != 0 )    // ｽﾚｯﾄﾞ終了指示ﾌﾗｸﾞでの終了指示がある場合はﾙｰﾌﾟ終了
	{
try {
		// 受信接続キュー内で、接続待ちがあるか判断
		if( server.Pending() == true )
		{
			// クライアントからの接続を受け付ける
			TcpClient ClientSocket = server.AcceptTcpClient();
			if( ClientSocket.Connected )
			{
				Console.WriteLine( "{0}: Network Client incoming >>>", this.GetPluginName());
				// 通信ストリームの取得
				NetworkStream stream = ClientSocket.GetStream();
				StreamReader reader = new StreamReader( stream, Encoding.UTF8 );

				// 切断されるまで受信する
				while( true )
				{
					string command = reader.ReadLine();
					if( command == null )
						break;
					Console.WriteLine( "{0}: Command={1} (len={2})",
						this.GetPluginName(), command, command.Length );
					if( 0 < command.Length )
						ProcessCommand( command );
				}

				Console.WriteLine( "{0}: Network Client <<< disconnected", this.GetPluginName());
				reader.Close();
				ClientSocket.Close();
			}
		}
		// 短時間だけ待機
		System.Threading.Thread.Sleep(1000);
	}
catch( Exception e ){
	Console.WriteLine( "{0}: TCP server: Exception: {1}", this.GetPluginName(), e.Message );
}
}
    }

	/***************************
		受信したコマンド文字列を解析
	***************************/
	private void ProcessCommand( string command )
	{
		// 現在エディット中のメイドさん
		Maid maid = GameMain.Instance.CharacterMgr.GetMaid( 0 );
		if( maid == null )
			return; // do nothing

		// キー名＝値 のハッシュ
        Hashtable hash = new System.Collections.Hashtable();
		// ひとつ以上の空白で区切られたフィールド
		string[] fields = command.Split( ' ' );
		foreach( string f in fields ){
			if( 0 == f.Length )
				continue;				// 連続した区切り文字で生成された空のフィールド
			// キー名＝値
			string[] kv = f.Trim().Split( '=' );
			if ( kv.Length < 2 || 0 == kv[0].Length)
				continue;				// 不正なキー名
			hash.Add( kv[0].ToLower(), kv[1] );
		}

		/******** MOTION：ポーズを指定する ********/
		if( hash.ContainsKey( "motion" )) {
			// 適当な名前にすると Exception 発生
			string value = (string) hash[ "motion" ];
			try {
				if( 0 < value.Length )
					LoadMotion( maid, value );
			}
			catch( Exception ex ) {
				Debug.Log( "RemoteControl.Plugin caught exception: " + ex.Message );
			}
		}

		/******** FACE：表情を指定する ********/
		if( hash.ContainsKey( "face" )){
			string value = (string) hash[ "face" ];
			if( hash.ContainsKey( "duration" )){
				// まだ：DURATION
			}
			if( 0 < value.Length )
				maid.FaceAnime( value, 1f, 0 );
		}

		/******** FACE_BLEND：表情オプション()を指定する ********/
		if( hash.ContainsKey( "face_blend" )){
			string value = (string) hash[ "face_blend" ];
			if( 0 < value.Length )
				maid.FaceBlend( value );
		}

		/******** VOICE：台詞をしゃべる ********/
		if( hash.ContainsKey( "voice" )){
			string value = (string) hash[ "voice" ];
			// まだ：FADEIN
			// まだ：LOOP
			if( 0 < value.Length )
				//f_fTimeを変えてみる->徐々にフェード【イン】していく。単位：秒。
				maid.AudioMan.LoadPlay( value, /*f_fTime=*/0.0f, /*f_bStreaming*/false, /*f_bLoop*/false );
		}

		/******** SE：効果音を鳴らす ********/
		if( hash.ContainsKey( "se" )){
			string value = (string) hash[ "se" ];
			if( 0 < value.Length )
				GameMain.Instance.SoundMgr.PlaySe( value, /*loop=*/false );
			// 調べる：loop=trueにした場合、止める方法
		}
	}

	/***************************
		ChangeMotion
	*/
		private void LoadMotion( Maid maid, String motionName )
		{
			try {
				// シーン編集インスタンス取得
				SceneEdit sceneEdit = UnityEngine.Object.FindObjectOfType<SceneEdit>();
				if( sceneEdit != null )
				{
					// モーション/声/背景情報取得
					SceneEdit.PVBInfo[] poses = sceneEdit.m_dicPose[ maid.Param.status.personal ];
					if( 0 < poses.Length )
					{
						SceneEdit.PVBInfo pvb = new SceneEdit.PVBInfo();
						pvb.texIcon = poses[ 0 ].texIcon;

						// 取得したモーション/声/背景情報の設定値をそのまま使う
						pvb.info = new SceneEditInfo.PoseVoiceBgInfo();
						pvb.info.angle = poses[ 0 ].info.angle;
						pvb.info.bAutoTwistShoulder = poses[ 0 ].info.bAutoTwistShoulder;
						pvb.info.bone = poses[ 0 ].info.bone;
						pvb.info.bStage = poses[ 0 ].info.bStage;
						pvb.info.distance = poses[ 0 ].info.distance;
						pvb.info.fRot = poses[ 0 ].info.fRot;
						pvb.info.stage = poses[ 0 ].info.stage;
						pvb.info.strFileName3 = poses[ 0 ].info.strFileName3;
						pvb.info.strIconFileName = poses[ 0 ].info.strIconFileName;
						pvb.info.strName = poses[ 0 ].info.strName;
						pvb.info.vPos = poses[ 0 ].info.vPos;
						// モーション名書き換え
						pvb.info.strFileName = motionName;

						// 表情名は空(現状を維持)
						pvb.info.strFileName2 = String.Empty;

						// モーション変更
						sceneEdit.Pose( pvb );
					}
				}
			}
			catch( Exception e ) {
				System.Windows.Forms.MessageBox.Show( e.ToString(), Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().Location ), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}

		/****************************************
			起動してプラグインが読み込まれたときに一度
		****************************************/
		public void Awake()
		{	//Console.WriteLine( "{0}::Awake", this.GetPluginName());
			try {
				GameObject.DontDestroyOnLoad( this );
			}
			catch( Exception e ) {
				Console.WriteLine( "{0}::Awale: Exception: {1}", this.GetPluginName(), e.Message );
			}
		}

		/****************************************
			シーンがきりわかったとき
		****************************************/
		public void OnLevelWasLoaded( int level )
		{	//Console.WriteLine( "{0}::OnLevelWasLoaded", this.GetPluginName());
			if( level == 5 && this.sceneNo != 5 )
				EnterSceneLv5();
			else if( level != 5 && this.sceneNo == 5 )
				LeaveSceneLv5();
			this.sceneNo = level;
		}

		private void EnterSceneLv5()
		{	Console.WriteLine( "{0}::EnterSceneLv5", this.GetPluginName());

			//******** サーバ開始 ********
			my_thread_flag = 1;
	        ListeningCallbackThread = new Thread( MyThreadProc );
	        ListeningCallbackThread.Start();
		}

		private void LeaveSceneLv5()
		{	Console.WriteLine( "{0}::LeaveSceneLv5", this.GetPluginName());

			//******** サーバ停止 ********
            if( my_thread_flag != 0 )        {
				if( server != null )
				{
                    // 接続要求受け入れの終了
                    server.Stop();
                    server = null;
				}
				my_thread_flag = 0;
				ListeningCallbackThread.Abort();
            }
		}

		///-------------------------------------------------------------------------
		/// <summary>プラグイン名取得</summary>
		/// <returns>プラグイン名</returns>
		///-------------------------------------------------------------------------
		private String GetPluginName()
		{
			String name = String.Empty;
			try {
				// 属性クラスからプラグイン名取得
				PluginNameAttribute att = Attribute.GetCustomAttribute( typeof( RemoteControl ), typeof( PluginNameAttribute )) as PluginNameAttribute;
				if( att != null )
					name = att.Name;
			}
			catch( Exception e ) {
				Console.WriteLine( "{0}::GetPluginName: Exception: {1}", this.GetPluginName(), e.Message );
			}
			return name;
		}

		///-------------------------------------------------------------------------
		/// <summary>プラグインバージョン取得</summary>
		/// <returns>プラグインバージョン</returns>
		///-------------------------------------------------------------------------
		private String GetPluginVersion()
		{
			String version = String.Empty;
			try {
				// 属性クラスからバージョン番号取得
				PluginVersionAttribute att = Attribute.GetCustomAttribute( typeof( RemoteControl ), typeof( PluginVersionAttribute ) ) as PluginVersionAttribute;
				if( att != null )
					version = att.Version;
			}
			catch( Exception e ) {
				Console.WriteLine( "{0}::GetPluginVersion: Exception: {1}", this.GetPluginName(), e.Message );
			}
			return version;
		}
		#endregion

		#region Fields
		/// <summary>画面番号</summary>
		private int sceneNo = 0;
		#endregion
	}
	#endregion
}