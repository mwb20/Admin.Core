﻿namespace ZhonTai.Plugin.Lazy.SlideCaptcha.Core.Exceptions;

public class SlideCaptchaTimeoutException : Exception
{
    public SlideCaptchaTimeoutException() : base()
    {
    }

    public SlideCaptchaTimeoutException(string message) : base(message)
    {
    }

    public SlideCaptchaTimeoutException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
