import React, { useState, useEffect } from "react";
import "../index.css";
import "../login.css";
import "../forget.css";
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import Navbar from "../components/Navbar";
import { authApi } from "../api/authApi";

import icon7 from "../assets/lock.png";

function ResetPassword() {
    const [searchParams] = useSearchParams();
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [loading, setLoading] = useState(false);
    const [tokenValid, setTokenValid] = useState(null); // null = checking
    const navigate = useNavigate();

    const token = searchParams.get("token");

    useEffect(() => {
        if (!token) {
            setTokenValid(false);
            return;
        }

        authApi.validateResetToken(token)
            .then(res => setTokenValid(res.data === true || res.success === true))
            .catch(() => setTokenValid(false));
    }, [token]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setSuccess("");

        if (!newPassword || !confirmPassword) {
            setError("Please fill in all fields.");
            return;
        }

        if (newPassword.length < 6) {
            setError("Password must be at least 6 characters long.");
            return;
        }

        if (newPassword !== confirmPassword) {
            setError("Passwords do not match.");
            return;
        }

        setLoading(true);
        try {
            await authApi.resetPassword(token, newPassword);
            setSuccess("Password reset successfully! Redirecting to login...");
            setTimeout(() => navigate("/login"), 2000);
        } catch (err) {
            setError(err.response?.data?.message || "Failed to reset password. The link may have expired.");
        } finally {
            setLoading(false);
        }
    };

    if (tokenValid === null) {
        return (
            <>
                <Navbar />
                <section className="login-section">
                    <div className="login-box">
                        <div className="login">
                            <p className="login-description">Validating reset link...</p>
                        </div>
                    </div>
                </section>
            </>
        );
    }

    if (tokenValid === false) {
        return (
            <>
                <Navbar />
                <section className="login-section">
                    <div className="login-box">
                        <div className="login-icon">
                            <img src={icon7} alt="lock icon" width={30} height={30} />
                        </div>
                        <div className="login">
                            <div className="login-title">
                                <h1 className="welcome">Invalid Reset Link</h1>
                                <p className="login-description">
                                    This password reset link is invalid or has expired. Please request a new one.
                                </p>
                            </div>
                            <div className="forget-pass" style={{ marginTop: "16px" }}>
                                <Link className="link" to="/forget">Request a new reset link</Link>
                            </div>
                        </div>
                    </div>
                </section>
            </>
        );
    }

    return (
        <>
            <Navbar />
            <section className="login-section">
                <div className="login-box">
                    <div className="login-icon">
                        <img src={icon7} alt="lock icon" width={30} height={30} />
                    </div>
                    <div className="login">
                        <div className="login-title">
                            <h1 className="welcome">Reset Your Password</h1>
                            <p className="login-description">
                                Enter your new password below.
                            </p>
                        </div>
                        {error && <div className="form-error-msg">{error}</div>}
                        {success && <div className="form-success-msg">{success}</div>}
                        <form className="login-form" onSubmit={handleSubmit}>
                            <h5 className="login-form-title">New Password</h5>
                            <div className="forget-input-wrapper" style={{ position: "relative" }}>
                                <i className="fa-solid fa-lock forget-input-icon"></i>
                                <input
                                    className="login-form-input"
                                    type={showPassword ? "text" : "password"}
                                    placeholder="Enter your new password"
                                    value={newPassword}
                                    onChange={(e) => setNewPassword(e.target.value)}
                                    disabled={loading}
                                    required
                                />
                                <i
                                    className={showPassword ? "fa-solid fa-eye-slash login-input-icon-right" : "fa-solid fa-eye login-input-icon-right"}
                                    onClick={() => setShowPassword(prev => !prev)}
                                    role="button"
                                    aria-label={showPassword ? "Hide password" : "Show password"}
                                    tabIndex={0}
                                    onKeyDown={(ev) => { if (ev.key === "Enter" || ev.key === " ") setShowPassword(p => !p); }}
                                ></i>
                            </div>
                            <p style={{ fontSize: "12px", color: "#888", marginBottom: "8px" }}>
                                Minimum 6 characters
                            </p>
                            <h5 className="login-form-title">Confirm New Password</h5>
                            <div className="forget-input-wrapper" style={{ position: "relative" }}>
                                <i className="fa-solid fa-lock forget-input-icon"></i>
                                <input
                                    className="login-form-input"
                                    type={showConfirmPassword ? "text" : "password"}
                                    placeholder="Confirm your new password"
                                    value={confirmPassword}
                                    onChange={(e) => setConfirmPassword(e.target.value)}
                                    disabled={loading}
                                    required
                                />
                                <i
                                    className={showConfirmPassword ? "fa-solid fa-eye-slash login-input-icon-right" : "fa-solid fa-eye login-input-icon-right"}
                                    onClick={() => setShowConfirmPassword(prev => !prev)}
                                    role="button"
                                    aria-label={showConfirmPassword ? "Hide password" : "Show password"}
                                    tabIndex={0}
                                    onKeyDown={(ev) => { if (ev.key === "Enter" || ev.key === " ") setShowConfirmPassword(p => !p); }}
                                ></i>
                            </div>
                            <button type="submit" className="btn-forget-primary" disabled={loading}>
                                {loading ? "Resetting..." : "Reset Password"}
                            </button>
                            <div className="forget-pass">
                                <Link className="link" to="/login">Back to Login</Link>
                            </div>
                        </form>
                    </div>
                </div>
            </section>
        </>
    );
}

export default ResetPassword;
