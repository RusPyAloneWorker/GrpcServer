import React, {useState} from "react";
import "../static/css/header.css"

export var Header = () => {
    const HEADER_DOWN = 150;
    const [isScrolled, handleScroll] = useState(window.scrollY > HEADER_DOWN);
    window.addEventListener("scroll", () => handleScroll(window.scrollY > HEADER_DOWN))

    return (
    <>
        <header className={`${isScrolled ? "scrolled" : ""}`}>
            <div className='header-content'>
                <div className="logo">
                    lol
                </div>
                <ul className="links">
                    <li className="sign-in-link">
                        <a>lol</a>
                    </li>
                    <li className="about-link">
                        <a>lol</a>
                    </li>
                </ul>
            </div>
            <script>
                window.addEventListener("scroll", () => ))
            </script>
        </header>
    </>
    )
}