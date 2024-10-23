﻿using DomainObjects.Pregnancy.Localizations;
using Hangfire;
using MailSenderLibrary.Interfaces;
using MailSenderLibrary.Models;
using Newtonsoft.Json.Linq;
using PaymentService.Services.Interfaces;

namespace PaymentService.Services.Implementations
{
    public class RobokassaMailService : IRobokassaMailService
    {
        private readonly IEmailService _mailService;
        private readonly ILogger<RobokassaMailService> _logger;

        public RobokassaMailService(IEmailService mailService, ILogger<RobokassaMailService> logger)
        {
            _mailService = mailService;
            _logger = logger;
        }

        [Queue("emails")]
        public Task SendRecurrentPaymentEmailAsync(string mailAddress, DateTime nextSubDate, string sum, string subName)
        {
            return _mailService.SendEmailAsync(GenerateRecurrentPaymentMessage(mailAddress, nextSubDate, sum, subName, LocalizationsLanguage.ru));
        }

        [Queue("emails")]
        public Task SendSuccessPaymentEmailAsync(string mailAddress, DateTime subStartDate, DateTime subEndDate, string subName)
        {
            return _mailService.SendEmailAsync(GenerateSuccessPaymentMessage(mailAddress, subStartDate, subEndDate, subName, LocalizationsLanguage.ru));
        }

        [Queue("emails")]
        public Task SendFailPaymentEmailAsync(string mailAddress)
        {
            throw new NotImplementedException();
        }

        private static EmailMessage GenerateRecurrentPaymentMessage(string email, DateTime nextSubDate, string sum, string subName, LocalizationsLanguage language)
        {
            var content = "";
            switch (language)
            {
                case LocalizationsLanguage.ru:
                    content = $"<!DOCTYPE html>\n\n<html lang=\"en\" xmlns=\"https://www.w3.org/1999/xhtml\">\n<head>\n    <meta charset=\"utf-8\" />\n    <title></title>\n</head>\n<body paddingwidth=\"0\" paddingheight=\"0\" \n    style=\"font-family: Roboto,Helvetica, Arial,serif; padding: 0; margin: 0;  width: 100% !important; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; background-color:#fff; text-align: center;\" offset=\"0\" toppadding=\"0\" leftpadding=\"0\">\n\n    <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"tableContent bgBody\" align=\"center\">\n        <tbody>\n            <tr>\n                <td>\n                    <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" class=\"bgItem\">\n                        <tbody style=\"border-left: 5px solid #fff; display: block; margin: 0 10px; border-right: 5px solid #fff;\">\n                            <tr>\n                                <td width=\"40\">&nbsp;</td>\n                                <td>\n                                    <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\n\n                                        <!-- =============================== Header ====================================== -->\n\n\n                                        <tbody>\n                                            <tr>\n                                                <td class=\"movableContentContainer\" valign=\"top\">\n                                                    <div class=\"movableContent\">\n                                                        <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding-top: 20px;\">\n                                                            <tbody>\n                                                                <tr>\n                                                                    <td valign=\"top\" align=\"center\">\n                                                                        <div class=\"contentEditableContainer contentTextEditable\">\n                                                                            <div class=\"contentEditable\">\n                                                                                <a href=\"https://babytips.me\">\n                                                                                    <img src=\"https://s3.eu-north-1.amazonaws.com/img-babytips.me/email/mail-logo.png\" alt=\"Baby Tips\">\n                                                                                </a>\n                                                                            </div>\n                                                                            <div style=\"height: 1px; background: #EAECF6;\"></div>\n                                                                        </div>\n                                                                    </td>\n                                                                </tr>\n                                                            </tbody>\n                                                        </table>\n                                                    </div>\n\n                                                    <div class=\"movableContent\">\n                                                        <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\n                                                            <tbody>\n\n                                                                <tr>\n                                                                    <td align=\"left\">\n                                                                        <div class=\"contentEditableContainer contentTextEditable\">\n                                                                            <div class=\"contentEditable\" align=\"left\" style=\"color:#222;text-align:left;font-size:14px;font-weight:normal;line-height:19px;\">\n                                                                                <h1 style=\"color: #272C4D; text-align: center; font-feature-settings: 'clig' off, 'liga' off; font-size: 28px; font-style: normal; font-weight: 400; line-height: 35px;\">\n                                                                                    Напоминание о продлении подписки\n                                                                                </h1>\n\n                                                                                <div style=\"text-align: center; margin: 24px 0 0;\">\n                                                                                    <a href=\"https://babytips.me\" style=\"padding: 9px 16px; display: inline-block; border-radius: 300px; background: #64A3FF; color: #fff; text-decoration: none; \">\n                                                                                        BabyTips.me\n                                                                                    </a>\n                                                                                </div>\n                                                                                <p style=\"text-align: center; margin: 10px 0 24px 0\">Мама и малыш теперь под надёжной защитой</p>\n\n                                                                                <div style=\"margin-bottom: 24px;\">\n                                                                                    <p>\n                                                                                        Хотели бы напомнить вам, что ваша подписка на наш сервис автоматически продлится через 2 недели. {nextSubDate.ToString("dd.MM.yyyy")},\n                                                                                        будет списана сумма в размере {sum} за следующий период пользования услугами.\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Детали подписки:\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Название подписки: {subName} \n                                                                                        <br/>\n                                                                                        Дата продления: {nextSubDate.ToString("dd.MM.yyyy")}\n                                                                                        <br />\n                                                                                        Сумма списания: {sum}\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Мы делаем всё возможное, чтобы вы могли продолжать пользоваться нашим сервисом без перебоев. Если у вас возникнут вопросы по\n                                                                                        поводу вашей подписки или вам понадобится помощь с оплатой, пожалуйста, не стесняйтесь обращаться в нашу службу\n                                                                                        поддержки по адресу\n                                                                                        <a href=\"mailto:info@babytips.me\" style=\"color: #3E8DFF; font-style: normal; font-weight: 400; text-decoration: none;\">info@babytips.me</a>.\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Вы можете отменить подписку, перейдя по ссылке ниже: \n                                                                                        <br/>\n                                                                                        <a href=\"#stop-link\" style=\"color: #3E8DFF; font-style: normal; font-weight: 400; text-decoration: none;\">ОТМЕНИТЬ ПОДПИСКУ</a>.\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Благодарим за использование нашего сервиса!\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        С наилучшими пожеланиями,\n                                                                                        <br />\n                                                                                        Команда BabyTips\n                                                                                    </p>\n                                                                                </div>\n                                                                            </div>\n                                                                        </div>\n                                                                    </td>\n                                                                </tr>\n                                                            </tbody>\n                                                        </table>\n                                                    </div>\n\n                                                    <!-- footer -->\n                                                    <div\n                                                        style=\"background-color:#fff; border-radius: 10px; padding: 20px 20px 0 20px; box-shadow: 0px 1px 3px 0px rgba(209, 211, 224, 0.60); text-align: left; color:#A0A5B9; font-size: 12px; line-height: 16px; \">\n                                                        <p style=\"margin: 0 0 10px 0;\">\n                                                            Вы получили это письмо, потому что этот адрес электронной почты был указан при регистрации на сервисе\n                                                            <a href=\"https://babytips.me\"\n                                                                style=\"color: #3E8DFF; font-style: normal; font-weight: 400; text-decoration: none;\">\n                                                                BabyTips.me\n                                                            </a>\n                                                            <br />\n                                                            Если это письмо доставлено вам по ошибке, просто удалите его, мы больше не будем вас беспокоить\n                                                        </p>\n                                                        <p style=\"margin: 0 0 10px 0;\">\n                                                            <a href=\"https://babytips.me\"\n                                                                style=\"color: #3E8DFF; font-style: normal; font-weight: 400; text-decoration: none;\">\n                                                                Отписаться от рассылки\n                                                            </a>\n                                                        </p>\n                                                        <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\n                                                            <tbody>\n                                                                <tr>\n                                                                    <td>\n                                                                        <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\n                                                                            <tbody>\n                                                                                <tr>\n                                                                                    <td valign=\"bottom\" align=\"left\" width=\"107\" style=\"vertical-align: bottom;\">\n                                                                                        <img src=\"https://s3.eu-north-1.amazonaws.com/img-babytips.me/email/mail-footer-left.png\"\n                                                                                            alt=\"\">\n                                                                                    </td>\n                                                                                    <td valign=\"bottom\" align=\"right\" style=\"vertical-align: bottom;\"></td>\n                                                                                    <td valign=\"bottom\" align=\"right\" width=\"142\" style=\"vertical-align: bottom;\">\n                                                                                        <img src=\"https://s3.eu-north-1.amazonaws.com/img-babytips.me/email/mail-footer-right.png\"\n                                                                                            alt=\"\">\n                                                                                    </td>\n                                                                                </tr>\n                                                                            </tbody>\n                                                                        </table>\n                                                                    </td>\n                                                                </tr>\n                                                            </tbody>\n                                                        </table>\n                                                    </div>\n\n                                                </td>\n                                            </tr>\n\n\n                                        </tbody>\n                                    </table>\n                                </td>\n                                <td width=\"40\"></td>\n                            </tr>\n                        </tbody>\n                    </table>\n                </td>\n            </tr>\n        </tbody>\n    </table>\n</body>\n</html>";
                    break;
                case LocalizationsLanguage.en:
                    content = "Уведомление об оплате на английском. ЗАВТРА.";
                    break;
                default:
                    content = "Уведомление об оплате. ЗАВТРА.";
                    break;
            }
            return new EmailMessage(
               new List<string> { email },
               "Уведомление о подписке",
               content);
        }

        private static EmailMessage GenerateSuccessPaymentMessage(string email, DateTime subStartDate, DateTime subEndDate, string subName, LocalizationsLanguage language)
        {
            var content = "";
            switch (language)
            {
                case LocalizationsLanguage.ru:
                    content = $"<!DOCTYPE html>\n\n<html lang=\"en\" xmlns=\"https://www.w3.org/1999/xhtml\">\n<head>\n    <meta charset=\"utf-8\" />\n    <title></title>\n</head>\n<body paddingwidth=\"0\" paddingheight=\"0\" \n    style=\"font-family: Roboto,Helvetica, Arial,serif; padding: 0; margin: 0;  width: 100% !important; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; background-color:#fff; text-align: center;\" offset=\"0\" toppadding=\"0\" leftpadding=\"0\">\n\n    <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"tableContent bgBody\" align=\"center\">\n        <tbody>\n            <tr>\n                <td>\n                    <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" class=\"bgItem\">\n                        <tbody style=\"border-left: 5px solid #fff; display: block; margin: 0 10px; border-right: 5px solid #fff;\">\n                            <tr>\n                                <td width=\"40\">&nbsp;</td>\n                                <td>\n                                    <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\n\n                                        <!-- =============================== Header ====================================== -->\n\n\n                                        <tbody>\n                                            <tr>\n                                                <td class=\"movableContentContainer\" valign=\"top\">\n                                                    <div class=\"movableContent\">\n                                                        <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding-top: 20px;\">\n                                                            <tbody>\n                                                                <tr>\n                                                                    <td valign=\"top\" align=\"center\">\n                                                                        <div class=\"contentEditableContainer contentTextEditable\">\n                                                                            <div class=\"contentEditable\">\n                                                                                <a href=\"https://babytips.me\">\n                                                                                    <img src=\"https://s3.eu-north-1.amazonaws.com/img-babytips.me/email/mail-logo.png\" alt=\"Baby Tips\">\n                                                                                </a>\n                                                                            </div>\n                                                                            <div style=\"height: 1px; background: #EAECF6;\"></div>\n                                                                        </div>\n                                                                    </td>\n                                                                </tr>\n                                                            </tbody>\n                                                        </table>\n                                                    </div>\n\n                                                    <div class=\"movableContent\">\n                                                        <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\n                                                            <tbody>\n\n                                                                <tr>\n                                                                    <td align=\"left\">\n                                                                        <div class=\"contentEditableContainer contentTextEditable\">\n                                                                            <div class=\"contentEditable\" align=\"left\" style=\"color:#222;text-align:left;font-size:14px;font-weight:normal;line-height:19px;\">\n                                                                                <h1 style=\"color: #272C4D; text-align: center; font-feature-settings: 'clig' off, 'liga' off; font-size: 28px; font-style: normal; font-weight: 400; line-height: 35px;\">\n                                                                                    Подтверждение успешной подписки\n                                                                                </h1>\n\n                                                                                <div style=\"text-align: center; margin: 24px 0 0;\">\n                                                                                    <a href=\"https://babytips.me\" style=\"padding: 9px 16px; display: inline-block; border-radius: 300px; background: #64A3FF; color: #fff; text-decoration: none; \">\n                                                                                        BabyTips.me\n                                                                                    </a>\n                                                                                </div>\n                                                                                <p style=\"text-align: center; margin: 10px 0 24px 0\">Мама и малыш теперь под надёжной защитой</p>\n\n                                                                                <div style=\"margin-bottom: 24px;\">\n                                                                                    <p>\n                                                                                        Мы рады сообщить вам об успешной подписке на премиум-план. Мы ценим ваш интерес к нашим услугам и рады приветствовать вас в\n                                                                                        нашем сообществе.\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Ваша подписка активирована с момента успешной транзакции, и вы можете начать пользоваться всеми возможностями нашего\n                                                                                        сервиса немедленно.\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Ниже вы найдете подробности вашей подписки:\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Название подписки: {subName}\n                                                                                        <br/>\n                                                                                        Срок действия: {subStartDate.ToString("dd.MM.yyyy")} - {subEndDate.ToString("dd.MM.yyyy")}\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Пожалуйста, обратите внимание на дату окончания вашей подписки, чтобы своевременно продлить ее и продолжить пользоваться\n                                                                                        нашими услугами без перерывов.\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Если у вас возникнут какие-либо вопросы или вам потребуется помощь, не стесняйтесь связаться с нашей службой поддержки\n                                                                                        по адресу \n                                                                                        <a href=\"mailto:info@babytips.me\" style=\"color: #3E8DFF; font-style: normal; font-weight: 400; text-decoration: none;\">info@babytips.me</a>.\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Вы можете отменить подписку, перейдя по ссылке ниже:\n                                                                                        <br />\n                                                                                        <a href=\"#stop-link\" style=\"color: #3E8DFF; font-style: normal; font-weight: 400; text-decoration: none;\">ОТМЕНИТЬ\n                                                                                            ПОДПИСКУ</a>.\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        Спасибо за ваш выбор нашего сервиса!\n                                                                                    </p>\n                                                                                    <p>\n                                                                                        С наилучшими пожеланиями,\n                                                                                        <br />\n                                                                                        Команда BabyTips\n                                                                                    </p>\n                                                                                </div>\n                                                                            </div>\n                                                                        </div>\n                                                                    </td>\n                                                                </tr>\n                                                            </tbody>\n                                                        </table>\n                                                    </div>\n\n                                                    <!-- footer -->\n                                                    <div style=\"background-color:#fff; border-radius: 10px; padding: 20px 20px 0 20px; box-shadow: 0px 1px 3px 0px rgba(209, 211, 224, 0.60); text-align: left; color:#A0A5B9; font-size: 12px; line-height: 16px; \">\n                                                        <p style=\"margin: 0 0 10px 0;\">\n                                                            Вы получили это письмо, потому что этот адрес электронной почты был указан при регистрации на сервисе\n                                                            <a href=\"https://babytips.me\"\n                                                                style=\"color: #3E8DFF; font-style: normal; font-weight: 400; text-decoration: none;\">\n                                                                BabyTips.me\n                                                            </a>\n                                                            <br/>\n                                                            Если это письмо доставлено вам по ошибке, просто удалите его, мы больше не будем вас беспокоить\n                                                        </p>\n                                                        <p style=\"margin: 0 0 10px 0;\">\n                                                            <a href=\"https://babytips.me\" style=\"color: #3E8DFF; font-style: normal; font-weight: 400; text-decoration: none;\">\n                                                                Отписаться от рассылки\n                                                            </a>\n                                                        </p>\n                                                        <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\n                                                            <tbody>\n                                                                <tr>\n                                                                    <td>\n                                                                        <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\n                                                                            <tbody>\n                                                                                <tr>\n                                                                                    <td valign=\"bottom\" align=\"left\" width=\"107\" style=\"vertical-align: bottom;\">\n                                                                                        <img src=\"https://s3.eu-north-1.amazonaws.com/img-babytips.me/email/mail-footer-left.png\" alt=\"\">\n                                                                                    </td>\n                                                                                    <td valign=\"bottom\" align=\"right\" style=\"vertical-align: bottom;\"></td>\n                                                                                    <td valign=\"bottom\" align=\"right\" width=\"142\" style=\"vertical-align: bottom;\">\n                                                                                        <img src=\"https://s3.eu-north-1.amazonaws.com/img-babytips.me/email/mail-footer-right.png\" alt=\"\">\n                                                                                    </td>\n                                                                                </tr>\n                                                                            </tbody>\n                                                                        </table>\n                                                                    </td>\n                                                                </tr>\n                                                            </tbody>\n                                                        </table>\n                                                    </div>\n\n                                                </td>\n                                            </tr>\n\n\n                                        </tbody>\n                                    </table>\n                                </td>\n                                <td width=\"40\"></td>\n                            </tr>\n                        </tbody>\n                    </table>\n                </td>\n            </tr>\n        </tbody>\n    </table>\n</body>\n</html>";
                    break;
                case LocalizationsLanguage.en:
                    content = "Успешный платеж по подписке на английском";
                    break;
                default:
                    content = "Успешный платеж по подписке";
                    break;
            }
            return new EmailMessage(
               new List<string> { email },
               "Успешный платеж по подписке!",
               content);
        }
    }
}
