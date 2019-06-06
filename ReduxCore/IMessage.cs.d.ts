declare module Server {
	interface Message {
		MsgType: Server.MessageType;
		Msg: string;
		Duration: number;
		Showtype: Server.Showtype;
	}
	const enum MessageType {
		Undefine,
		Waring = 0,
		Success = 1,
		Info = 2,
		Error = 3,
	}
	const enum Showtype {
		Message = 0,
		Alert = 1,
		Loading = 2,
		MessageBox = 3,
		Notification = 4,
	}
}
