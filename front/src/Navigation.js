import profile from "./profile.svg";
import "./Navigation.css";
import React from "react";

import {Link} from "react-router-dom";

export default function Navigation() {
    console.log(profile)

    return (
        <div className="Navigation">
            <nav>
                <Link id="dormitories" to="/">
                    Общежития
                </Link>
                <input id='search' type="text"/>
                <img id="profile" src={profile} alt="Профиль"/>
            </nav>
        </div>
    )
}