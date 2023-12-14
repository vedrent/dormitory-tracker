import "../styles/Authorization.css"
import {useState} from "react";
import Navigation from "./Navigation";
import Toolbar from "./Toolbar";

export default function Authorization(props) {

    const [emailState, setEmailState] = useState(null)
    const [passwordState, setPasswordState] = useState(null)

    return (
        <div className="Authorization">
            <Navigation/>
            <section>
                <Toolbar/>

                <form method='post' action="/login">
                    <p>
                        <label>Введите ваш логин</label><br />
                        <input name='login' type='text'
                               onChange={event => setEmailState(event.target.value)}/>
                    </p>
                    <p>
                        <label>Введите ваш пароль</label><br />
                        <input name='password' type='password'
                               onChange={event => setPasswordState(event.target.value)}/>
                    </p>
                    <input type='submit' value='Войти в аккаунт' />
                </form>
            </section>
        </div>
    )
}