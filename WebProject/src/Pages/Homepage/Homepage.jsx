import '../../static/css/homepage.css';
import {useEffect, useState} from "react";

export let Homepage = () => {
    const [text, setText] = useState("");
    const [messages, setMessages] = useState([]);
    const [user, setUser] = useState("");
    const [pass, setPass] = useState("");
    const { ChatClient } = require("../../proto/chat_grpc_web_pb");
    const { JoinChatRequest, Message } = require("../../proto/chat_pb");
    const metadata =  {"Authorization" : `Bearer ${localStorage?.getItem('jwt')?.replaceAll('"','')}`}

    const service = new ChatClient("http://localhost:10000");

    useEffect(() => {
        (async () => {
            let request = new JoinChatRequest();
            request.setMessage("message");
            let result = service.joinChat(request, metadata, (err, response) => {
                if (err) {
                    console.log(err.message);
                }
                else {
                    console.log(response.getMessage());
                }
            });
            result.on("data", function(reply){
                console.log(reply.getMessage());
                setMessages((messages) => [...messages, reply.getMessage()]);
            });
        }) ();
    }, [])

    const handleSend = () => {
        if (!text || text.length === 0) {
            alert("Сообщение пустое");
            return;
        }
        let mess = new Message();
        mess.setMessage(text);
        service.sendMessage(mess, metadata,(err, response) => {
            if (response == null) {
                console.log(err)
            } else {
                console.log(response)
            }
        });
    }

    const handleAuth = () => {
        const { JwtClient } = require("../../proto/jwt_grpc_web_pb");
        const { JwtRequest } = require("../../proto/jwt_pb");

        let jwtService = new JwtClient("http://localhost:10000");
        let jwtRequest = new JwtRequest();
        jwtRequest.setUser(user);
        jwtRequest.setPassword(pass);

        jwtService.authorize(jwtRequest, metadata, function (err, response){
            if (err) {
                console.log(err.code);
                console.log(err.message);
            } else {
                let token = response.getToken();
                console.log(token);
                localStorage.setItem('jwt', JSON.stringify(token));
            }
        });
    }

    return (
        <>
            <section className="greeting-section">
                <div className="greeting-section-image" />
            </section>
            <section className="about">
                <div className="about-text"></div>
                {/*<img className="about-sport_car_image" src={sport_car_img} />*/}
            </section>
            <section className={"chat"}>

                <input onChange={(e) => setUser(e.target.value)} placeholder={"name"} className={"text-input"} type={"text"}></input>
                <input onChange={(e) => setPass(e.target.value)} placeholder={"password"} className={"text-input"} type={"text"}></input>
                <button
                    onClick={handleAuth}
                    style={{position:"relative", width:"600px", height:"30px", margin:"10px", marginBottom:"30px"}}>
                    Нажми, чтобы авторизоваться
                </button>

                <h1 style={{color:"white", fontSize:"xxx-large"}}>Чат</h1>
                <div className={"messages"}>
                    <div className={"message"}>Сообщение</div>
                    {messages.map((message) => (
                        <div className={"message"}>{message}</div>
                    ))}
                </div>
                <input onChange={(e) => setText(e.target.value)} className={"text-input"} type={"text"}></input>
                <button
                    onClick={handleSend}
                    style={{position:"relative", width:"600px", height:"30px", margin:"10px"}}>
                    Нажми, чтобы отправить grpc
                </button>
            </section>
        </>
    );
};