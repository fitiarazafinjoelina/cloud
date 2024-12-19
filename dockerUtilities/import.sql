--
-- PostgreSQL database dump
--

-- Dumped from database version 15.2
-- Dumped by pg_dump version 15.2

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
-- Name: user_cloud; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_cloud (
    id_user_cloud integer NOT NULL,
    email character varying(255) NOT NULL,
    username character varying(255) NOT NULL,
    password character varying(255) NOT NULL,
    nb_tentative integer NOT NULL
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
\.


--
-- Data for Name: temporary_token; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.temporary_token (id_token, token, date_debut, date_fin, id_user) FROM stdin;
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
\.


--
-- Data for Name: user_cloud; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_cloud (id_user_cloud, email, username, password, nb_tentative) FROM stdin;
\.


--
-- Data for Name: user_validation; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_validation (id, username, email, password) FROM stdin;
\.


--
-- Name: pin_id_pin_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.pin_id_pin_seq', 1, false);


--
-- Name: temporary_token_id_token_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.temporary_token_id_token_seq', 1, false);


--
-- Name: temporary_uniqid_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.temporary_uniqid_id_seq', 1, false);


--
-- Name: token_id_token_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.token_id_token_seq', 1, false);


--
-- Name: user_cloud_id_user_cloud_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_cloud_id_user_cloud_seq', 1, false);


--
-- Name: user_validation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_validation_id_seq', 1, false);


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

