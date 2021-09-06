using Game.Model;
using System;

namespace Game.Core.Endpoints
{
    public class LogItem
    {
        public Endpoint From;
        public Endpoint Too;
        public LogType LogType;
        public AccessLevel AccessLevel;
        public string UserName;
        public DateTime TimeStamp;
        public string FilePath;

        public BankEndpoint FromBank;
        public BankEndpoint TooBank;
        public Person TransferFrom;
        public Person TransferToo;
        public int TransferAmount;
    }

    public class LogItemBuilder :
        ILogBuilder,
        ICONNECTION_ATTEMPT,
        ICONNECTION_FAILED,
        ICONNECTION_SUCCES,
        IFILE_EDITED,
        IFILE_COPIED,
        IFILE_DELETED,
        IFILE_RUN,
        ICONNECTION_ROUTED,
        ICONNECTION_DISCONNECTED,
        ICREDITTRANSFER,
        IFrom,
        IBetween,
        IUser,
        IAccesLevel
    {
        private Endpoint _From;
        private Endpoint _Too;
        private LogType _LogType;
        private AccessLevel _AccesLevel;
        private string _UserName;
        private DateTime _TimeStamp;
        private string _FilePath;


        private BankEndpoint _TransferFromBank;
        private BankEndpoint _TransferTooBank;
        private Person _TransferFrom;
        private Person _TransferToo;
        private int _TransferAmount;

        public static ILogBuilder Builder()
        {
            return new LogItemBuilder();
        }

        public IAccesLevel AccesLevel(AccessLevel accessLevel)
        {
            this._AccesLevel = accessLevel;
            return this;
        }

        public IBetween Between(Endpoint from, Endpoint too)
        {
            this._From = from;
            this._Too = too;
            return this;
        }

        public ICONNECTION_ATTEMPT CONNECTION_ATTEMPT()
        {
            this._LogType = LogType.CONNECTION_ATTEMPT;
            return this;
        }

        public ICONNECTION_DISCONNECTED CONNECTION_DISCONNECTED()
        {
            this._LogType = LogType.CONNECTION_DISCONNECTED;
            return this;
        }

        public ICONNECTION_FAILED CONNECTION_FAILED()
        {
            this._LogType = LogType.CONNECTION_FAILED;
            return this;
        }

        public ICONNECTION_ROUTED CONNECTION_ROUTED()
        {
            this._LogType = LogType.CONNECTION_ROUTED;
            return this;
        }

        public ICONNECTION_SUCCES CONNECTION_SUCCES()
        {
            this._LogType = LogType.CONNECTION_SUCCES;
            return this;
        }

        public ICREDITTRANSFER CREDITTRANSFER(Person from, BankEndpoint fromBank, Person too, BankEndpoint tooBank, int amount)
        {
            this._LogType = LogType.CREDITTRANSFER;
            this._TransferFrom = from;
            this._TransferToo = too;
            this._TransferAmount = amount;
            this._TransferFromBank = fromBank;
            this._TransferTooBank = tooBank;
            return this;
        }

        public IFILE_COPIED FILE_COPIED()
        {
            this._LogType = LogType.FILE_COPIED;
            return this;
        }

        public IFILE_DELETED FILE_DELETED()
        {
            this._LogType = LogType.FILE_DELETED;
            return this;
        }

        public IFILE_EDITED FILE_EDITED()
        {
            this._LogType = LogType.FILE_EDITED;
            return this;
        }

        public IFILE_RUN FILE_RUN(string path)
        {
            this._LogType = LogType.FILE_RUN;
            this._FilePath = path;
            return this;
        }

        public IFrom From(Endpoint from)
        {
            this._From = from;
            return this;
        }

        public LogItem TimeStamp(DateTime timeStamp)
        {
            this._TimeStamp = timeStamp;
            return new LogItem()
            {
                AccessLevel = this._AccesLevel,
                From = this._From,
                Too = this._Too,
                LogType = this._LogType,
                UserName = this._UserName,
                TimeStamp = this._TimeStamp,
                FilePath = this._FilePath,

                TooBank = this._TransferTooBank,
                FromBank = this._TransferFromBank,
                TransferAmount = this._TransferAmount,
                TransferFrom = this._TransferFrom,
                TransferToo = this._TransferToo
            };
        }

        public IUser User(string user)
        {
            this._UserName = user;
            return this;
        }
    }

    public interface ILogBuilder
    {
        public ICONNECTION_ATTEMPT CONNECTION_ATTEMPT();

        public ICONNECTION_FAILED CONNECTION_FAILED();

        public ICONNECTION_SUCCES CONNECTION_SUCCES();

        public IFILE_EDITED FILE_EDITED();

        public IFILE_COPIED FILE_COPIED();

        public IFILE_DELETED FILE_DELETED();

        public IFILE_RUN FILE_RUN(string path);

        public ICONNECTION_ROUTED CONNECTION_ROUTED();

        public ICONNECTION_DISCONNECTED CONNECTION_DISCONNECTED();

        public ICREDITTRANSFER CREDITTRANSFER(Person from, BankEndpoint fromBank, Person too, BankEndpoint tooBank, int amount);
    }

    public interface ICONNECTION_DISCONNECTED
    {
        public IFrom From(Endpoint from);
    }

    public interface ICONNECTION_ROUTED
    {
        public IBetween Between(Endpoint from, Endpoint too);
    }

    public interface IFILE_DELETED
    {
        public IFrom From(Endpoint from);
    }

    public interface IFILE_COPIED
    {
        public IFrom From(Endpoint from);
    }

    public interface IFILE_EDITED
    {
        public IFrom From(Endpoint from);
    }

    public interface IFILE_RUN
    {
        public IFrom From(Endpoint from);
    }

    public interface ICONNECTION_SUCCES
    {
        public IFrom From(Endpoint from);
    }

    public interface ICONNECTION_FAILED
    {
        public IFrom From(Endpoint from);
    }

    public interface ICONNECTION_ATTEMPT
    {
        public IFrom From(Endpoint from);
    }

    public interface ICREDITTRANSFER
    {
        public IFrom From(Endpoint from);
    }

    public interface IFrom
    {
        public IUser User(string user);
    }

    public interface IBetween
    {
        public IUser User(string user);
    }

    public interface IUser
    {
        public IAccesLevel AccesLevel(AccessLevel accessLevel);
    }

    public interface IAccesLevel
    {
        public LogItem TimeStamp(DateTime timeStamp);
    }
}