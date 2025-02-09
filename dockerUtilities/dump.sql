--
-- PostgreSQL database dump
--

-- Dumped from database version 15.10 (Debian 15.10-1.pgdg120+1)
-- Dumped by pg_dump version 15.10 (Debian 15.10-1.pgdg120+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: deduplicate_database(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.deduplicate_database() RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
    table_record RECORD;
    pk_columns TEXT;
    non_pk_columns TEXT;
    dynamic_sql TEXT;
BEGIN
    -- Loop through all tables in the database
    FOR table_record IN
        SELECT table_name
        FROM information_schema.tables
        WHERE table_schema = 'public' AND table_type = 'BASE TABLE'
        LOOP
            -- Get primary key columns (if any)
            SELECT string_agg(a.attname, ', ')
            INTO pk_columns
            FROM pg_index i
                     JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey)
            WHERE i.indrelid = (table_record.table_name)::regclass AND i.indisprimary;

            -- Get all non-PK columns
            SELECT string_agg(column_name, ', ')
            INTO non_pk_columns
            FROM information_schema.columns
            WHERE table_name = table_record.table_name
              AND column_name NOT IN (
                SELECT unnest(string_to_array(pk_columns, ', '))
            );

            -- Skip tables with no non-PK columns (e.g., tables with only a PK)
            IF non_pk_columns IS NULL THEN
                CONTINUE;
            END IF;

            -- Build dynamic SQL to delete duplicates
            dynamic_sql := format('
      DELETE FROM %I
      WHERE ctid IN (
        SELECT ctid
        FROM (
          SELECT 
            ctid,
            ROW_NUMBER() OVER (PARTITION BY %s ORDER BY ctid) AS rn
          FROM %I
        ) sub
        WHERE rn > 1
      )',
                                  table_record.table_name,
                                  non_pk_columns,
                                  table_record.table_name
                           );

            -- Execute the deduplication
            EXECUTE dynamic_sql;
        END LOOP;
END;
$$;


ALTER FUNCTION public.deduplicate_database() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: pin; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.pin (
    id_pin integer NOT NULL,
    id_user integer NOT NULL,
    pin_number integer NOT NULL,
    date_debut timestamp(0) without time zone NOT NULL,
    date_fin timestamp(0) without time zone NOT NULL
);


ALTER TABLE public.pin OWNER TO postgres;

--
-- Name: pin_id_pin_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.pin_id_pin_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.pin_id_pin_seq OWNER TO postgres;

--
-- Name: pin_id_pin_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.pin_id_pin_seq OWNED BY public.pin.id_pin;


--
-- Name: temporary_token; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.temporary_token (
    id_token integer NOT NULL,
    token character varying(255) NOT NULL,
    date_debut timestamp(0) without time zone NOT NULL,
    date_fin timestamp(0) without time zone NOT NULL,
    id_user integer NOT NULL
);


ALTER TABLE public.temporary_token OWNER TO postgres;

--
-- Name: temporary_token_id_token_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.temporary_token_id_token_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.temporary_token_id_token_seq OWNER TO postgres;

--
-- Name: temporary_token_id_token_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.temporary_token_id_token_seq OWNED BY public.temporary_token.id_token;


--
-- Name: temporary_uniqid; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.temporary_uniqid (
    id integer NOT NULL,
    uniqid character varying(255) NOT NULL,
    date_debut timestamp(0) without time zone NOT NULL,
    date_fin timestamp(0) without time zone NOT NULL,
    id_user integer NOT NULL
);


ALTER TABLE public.temporary_uniqid OWNER TO postgres;

--
-- Name: temporary_uniqid_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.temporary_uniqid_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.temporary_uniqid_id_seq OWNER TO postgres;

--
-- Name: temporary_uniqid_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.temporary_uniqid_id_seq OWNED BY public.temporary_uniqid.id;


--
-- Name: token; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.token (
    id_token integer NOT NULL,
    token character varying(255) NOT NULL,
    date_debut timestamp(0) without time zone NOT NULL,
    date_fin timestamp(0) without time zone NOT NULL,
    id_user integer NOT NULL
);


ALTER TABLE public.token OWNER TO postgres;

--
-- Name: token_id_token_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.token_id_token_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.token_id_token_seq OWNER TO postgres;

--
-- Name: token_id_token_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.token_id_token_seq OWNED BY public.token.id_token;


--
-- Name: user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."user" (
    id_user integer NOT NULL,
    email character varying(255) NOT NULL,
    username character varying(255) NOT NULL,
    password character varying(255) NOT NULL,
    id_role integer NOT NULL
);


ALTER TABLE public."user" OWNER TO postgres;

--
-- Name: user_cloud; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_cloud (
    id_user_cloud integer NOT NULL,
    email character varying(255) NOT NULL,
    username character varying(255) NOT NULL,
    password character varying(255) NOT NULL,
    nb_tentative integer NOT NULL,
    url_photo character varying(250) DEFAULT 'default'::character varying
);


ALTER TABLE public.user_cloud OWNER TO postgres;

--
-- Name: user_cloud_id_user_cloud_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.user_cloud_id_user_cloud_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.user_cloud_id_user_cloud_seq OWNER TO postgres;

--
-- Name: user_cloud_id_user_cloud_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.user_cloud_id_user_cloud_seq OWNED BY public.user_cloud.id_user_cloud;


--
-- Name: user_id_user_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.user_id_user_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.user_id_user_seq OWNER TO postgres;

--
-- Name: user_id_user_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.user_id_user_seq OWNED BY public."user".id_user;


--
-- Name: user_validation; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_validation (
    id integer NOT NULL,
    username character varying(255) NOT NULL,
    email character varying(255) NOT NULL,
    password character varying(255) NOT NULL
);


ALTER TABLE public.user_validation OWNER TO postgres;

--
-- Name: user_validation_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.user_validation_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.user_validation_id_seq OWNER TO postgres;

--
-- Name: user_validation_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.user_validation_id_seq OWNED BY public.user_validation.id;


--
-- Name: pin id_pin; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pin ALTER COLUMN id_pin SET DEFAULT nextval('public.pin_id_pin_seq'::regclass);


--
-- Name: temporary_token id_token; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temporary_token ALTER COLUMN id_token SET DEFAULT nextval('public.temporary_token_id_token_seq'::regclass);


--
-- Name: temporary_uniqid id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temporary_uniqid ALTER COLUMN id SET DEFAULT nextval('public.temporary_uniqid_id_seq'::regclass);


--
-- Name: token id_token; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.token ALTER COLUMN id_token SET DEFAULT nextval('public.token_id_token_seq'::regclass);


--
-- Name: user id_user; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user" ALTER COLUMN id_user SET DEFAULT nextval('public.user_id_user_seq'::regclass);


--
-- Name: user_cloud id_user_cloud; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_cloud ALTER COLUMN id_user_cloud SET DEFAULT nextval('public.user_cloud_id_user_cloud_seq'::regclass);


--
-- Name: user_validation id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_validation ALTER COLUMN id SET DEFAULT nextval('public.user_validation_id_seq'::regclass);


--
-- Data for Name: pin; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.pin (id_pin, id_user, pin_number, date_debut, date_fin) FROM stdin;
26	10	431209	2025-02-09 12:30:13	2025-02-09 12:31:43
\.


--
-- Data for Name: temporary_token; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.temporary_token (id_token, token, date_debut, date_fin, id_user) FROM stdin;
29	3-6lVfsVVvOZmGwiKI-o7joB3k8s1GpId8ki844yjoU	2025-02-09 12:30:12	2025-02-09 13:30:12	10
\.


--
-- Data for Name: temporary_uniqid; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.temporary_uniqid (id, uniqid, date_debut, date_fin, id_user) FROM stdin;
\.


--
-- Data for Name: token; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.token (id_token, token, date_debut, date_fin, id_user) FROM stdin;
100	jBbEKDPkx0fvCJBt0kCq7XUhge3TzYIvqSidwu8MAF0	2025-02-09 12:30:31	2025-02-09 13:30:31	10
101	223bf0d3-a8a4-4b4b-a969-679b637f4304	2025-02-09 12:41:53	2025-02-16 12:41:53	10
\.


--
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."user" (id_user, email, username, password, id_role) FROM stdin;
\.


--
-- Data for Name: user_cloud; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_cloud (id_user_cloud, email, username, password, nb_tentative, url_photo) FROM stdin;
8	nyavorandrianarisoa@gmail.com	Ny Avo	pOfIQXXWnHiFeqCmYZzXtejO9pY=|pPMqc+iwtf6mxijlXj02nqxe/6NfNjNJx1jgOFf8IpM=	0	1739044009544_photo_1739044009534.jpg
10	mpitsara123@gmail.com	Mpitsara	wyM4iFhhCShP/pt2FA2OxaJjAuE=|nTRFxFGH1HBeqF8QofmdQMn0chQQTuAB5+oDQu8bdmQ=	0	1739105000963_photo_1739105000951.jpg
\.


--
-- Data for Name: user_validation; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_validation (id, username, email, password) FROM stdin;
\.


--
-- Name: pin_id_pin_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.pin_id_pin_seq', 26, true);


--
-- Name: temporary_token_id_token_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.temporary_token_id_token_seq', 29, true);


--
-- Name: temporary_uniqid_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.temporary_uniqid_id_seq', 1, true);


--
-- Name: token_id_token_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.token_id_token_seq', 101, true);


--
-- Name: user_cloud_id_user_cloud_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_cloud_id_user_cloud_seq', 10, true);


--
-- Name: user_id_user_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_id_user_seq', 1, false);


--
-- Name: user_validation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_validation_id_seq', 10, true);


--
-- Name: pin pin_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pin
    ADD CONSTRAINT pin_pkey PRIMARY KEY (id_pin);


--
-- Name: temporary_token temporary_token_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temporary_token
    ADD CONSTRAINT temporary_token_pkey PRIMARY KEY (id_token);


--
-- Name: temporary_uniqid temporary_uniqid_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temporary_uniqid
    ADD CONSTRAINT temporary_uniqid_pkey PRIMARY KEY (id);


--
-- Name: token token_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.token
    ADD CONSTRAINT token_pkey PRIMARY KEY (id_token);


--
-- Name: user_cloud user_cloud_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_cloud
    ADD CONSTRAINT user_cloud_pkey PRIMARY KEY (id_user_cloud);


--
-- Name: user_cloud user_cloud_uniq; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_cloud
    ADD CONSTRAINT user_cloud_uniq UNIQUE (email);


--
-- Name: user user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_pkey PRIMARY KEY (id_user);


--
-- Name: user_validation user_validation_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_validation
    ADD CONSTRAINT user_validation_pkey PRIMARY KEY (id);


--
-- Name: pin pin_id_user_foreign; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pin
    ADD CONSTRAINT pin_id_user_foreign FOREIGN KEY (id_user) REFERENCES public.user_cloud(id_user_cloud);


--
-- Name: temporary_token temp_token_id_user_foreign; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temporary_token
    ADD CONSTRAINT temp_token_id_user_foreign FOREIGN KEY (id_user) REFERENCES public.user_cloud(id_user_cloud);


--
-- Name: temporary_uniqid temp_token_id_user_foreign; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temporary_uniqid
    ADD CONSTRAINT temp_token_id_user_foreign FOREIGN KEY (id_user) REFERENCES public.user_cloud(id_user_cloud);


--
-- Name: token token_id_user_foreign; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.token
    ADD CONSTRAINT token_id_user_foreign FOREIGN KEY (id_user) REFERENCES public.user_cloud(id_user_cloud);


--
-- PostgreSQL database dump complete
--

