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
2	7	323059	2024-12-17 14:09:51	2024-12-17 14:11:21
3	7	968198	2024-12-17 14:10:09	2024-12-17 14:11:39
4	7	674494	2024-12-17 14:12:28	2024-12-17 14:13:58
5	7	834352	2024-12-17 14:14:05	2024-12-17 14:15:35
6	7	29037	2024-12-17 14:15:35	2024-12-17 14:17:05
7	7	111426	2024-12-17 16:28:24	2024-12-17 16:29:54
8	7	290686	2024-12-17 16:29:29	2024-12-17 16:30:59
9	7	300712	2024-12-17 16:50:05	2024-12-17 16:51:35
10	7	330049	2024-12-17 16:51:47	2024-12-17 16:53:17
11	7	609199	2024-12-17 16:54:29	2024-12-17 16:55:59
12	7	354350	2024-12-17 16:55:28	2024-12-17 16:56:58
13	7	411792	2024-12-17 17:04:42	2024-12-17 17:06:12
14	7	323485	2024-12-17 17:08:02	2024-12-17 17:09:32
15	7	871863	2024-12-17 17:08:27	2024-12-17 17:09:57
16	7	368853	2024-12-17 17:12:57	2024-12-17 17:14:27
17	7	921204	2024-12-17 17:17:55	2024-12-17 17:19:25
18	7	796396	2024-12-18 12:42:56	2024-12-18 12:44:26
\.


--
-- Data for Name: temporary_token; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.temporary_token (id_token, token, date_debut, date_fin, id_user) FROM stdin;
3	/08RaCTKJhfa84SjrZDomqmSpQatXd9UvbBcsJv6WVM=	2024-12-17 14:09:51	2024-12-17 15:09:51	7
4	DTIbQjyqCf/RsI0GyG8zPwX36Bl2RgGEnK6SkK6wn3w=	2024-12-17 14:10:09	2024-12-17 15:10:09	7
5	/AVaQ3pMoTIbgGPOrMGMebHbBUN9GQIDp8BOXu58gEU=	2024-12-17 14:12:28	2024-12-17 15:12:28	7
6	GxxuAHsF/RHCIJk3DVAXx29/72M7pJ5qyAAzo+4QgWc=	2024-12-17 14:14:05	2024-12-17 15:14:05	7
7	HuDOi1KX8QH2iW7eWWyJfR6NSIhvctVzNIdn72V7uwA	2024-12-17 14:15:35	2024-12-17 15:15:35	7
8	HBHn5ol3PN2RKHkpmJ1ZgOvYp7qV_hCXBqXF2Enfhqo	2024-12-17 16:28:24	2024-12-17 17:28:24	7
9	W3mw3er1aIMTn3GLQhIZmYD95LuE5jI_eiWhqisD-gE	2024-12-17 16:29:29	2024-12-17 17:29:29	7
10	E6Et0KYPzlZWZRX8qBFeBMRks3c7lP1UUYk6RRuXJLQ	2024-12-17 16:50:05	2024-12-17 17:50:05	7
11	RnCjQph92HWGqrza9WkRGpoeHyVZ35aynYCowKlgbI0	2024-12-17 16:51:47	2024-12-17 17:51:47	7
12	OoejSLJWxXnbDIvWzSnM7FPKtFqt2KWTMsjvune5tOM	2024-12-17 16:54:29	2024-12-17 17:54:29	7
13	I9lwCCvwuisHJ8WOBOKWqAehp0FNIFhmgD3wsV07yL0	2024-12-17 16:55:28	2024-12-17 17:55:28	7
14	uIBoUnWp50-0wY1Z0yq1vjk5ly4ivYNZIuxXmAqpVig	2024-12-17 17:04:41	2024-12-17 18:04:41	7
15	tOTzUtDF7mNaoZ3VqN8Ay69EsSBEQFQbNkQJ71kMcsk	2024-12-17 17:08:02	2024-12-17 18:08:02	7
16	VqhI8yAZrpgn84q7Fjylj_BZOT0E29Ve1GyFIx1yxaU	2024-12-17 17:08:27	2024-12-17 18:08:27	7
17	kG67YIfO00v8Tgi6HZYTK3vrzgHg9lghuCuKcsd7dEg	2024-12-17 17:12:56	2024-12-17 18:12:56	7
18	ZRQfuQ60mcRNW4v8QbBcd4F6CwLZPFAugb_IJ7Pv468	2024-12-17 17:17:54	2024-12-17 18:17:54	7
19	4bWizAQpqgduiqg0LUAZvieCcm1ozgKoK5AzfpEsTzQ	2024-12-17 17:48:17	2024-12-17 18:48:17	7
20	9idC7rbKIWRh0VaRBafxFvIC4OMmBBc3VBd5PayUbvk	2024-12-17 17:49:28	2024-12-17 18:49:28	7
21	OyPhEFUPVi6ncSCgH7xUzF2g50oCwq2qXDhqwdsayP0	2024-12-18 12:42:56	2024-12-18 13:42:56	7
\.


--
-- Data for Name: temporary_uniqid; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.temporary_uniqid (id, uniqid, date_debut, date_fin, id_user) FROM stdin;
1	bZBfP1qTQ4So37n0XyysYY7clXs-wH5ipvtpKxh9hbQ	2024-12-18 12:44:26	2024-12-18 12:47:35	7
\.


--
-- Data for Name: token; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.token (id_token, token, date_debut, date_fin, id_user) FROM stdin;
2	pqrs7890uvwx1234yzab5678cdef9012	2024-12-17 11:00:00	2024-12-18 11:00:00	2
3	ijkl4567mnop8901qrst2345uvwx6789	2024-12-17 12:00:00	2024-12-18 12:00:00	3
4	mnop1234qrst5678uvwx9012yzab3456	2024-12-17 13:00:00	2024-12-18 13:00:00	4
5	abcd6789efgh0123ijkl3456mnop7890	2024-12-17 14:00:00	2024-12-18 14:00:00	5
1	abcd1234efgh5678ijkl9012mnop3456	2024-12-17 10:00:00	2024-12-17 10:00:00	1
6	RUTRSTrF7EiElH6O5BHMluCEZVx5V4jNKBNId16F_i8	2024-12-17 17:05:08	2024-12-17 18:05:08	7
7	hIhdGZOdkxjFnCVqS-B_iV0Oqfx1NQ4jxLAWo0vmLM4	2024-12-17 17:09:00	2024-12-17 18:09:00	7
8	wpNAJrLNCyIRAZAcQ7FZx7_jvppAICy9TBCYagp0zAI	2024-12-17 17:13:16	2024-12-17 18:13:16	7
9	62sfyqe7DNdFePRt8cHPiBzt2fQyqdtZHkXWarsPWFU	2024-12-17 17:18:18	2024-12-17 18:18:18	7
10	j9qZ9tIgh3HQR3viOW9hGLj_9ib0sh5VRIFmQ8N-aaU	2024-12-18 12:43:33	2024-12-18 13:43:33	7
\.


--
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."user" (id_user, email, username, password, id_role) FROM stdin;
\.


--
-- Data for Name: user_cloud; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_cloud (id_user_cloud, email, username, password, nb_tentative) FROM stdin;
1	alice@example.com	alice	password123	0
2	bob@example.com	bob	securePass456	0
3	charlie@example.com	charlie	myP@ssw0rd	0
4	david@example.com	david	Password987	0
5	emma@example.com	emma	em12345password	0
7	nyavorandrianarisoa@gmail.com	Test	LD8y1tBPxbUrTuH/K0DxiMzigSQ=|C1OtryHlHJaI6BciHNs3Z8YucANje6zZz3Ns1enmzvk=	0
\.


--
-- Data for Name: user_validation; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_validation (id, username, email, password) FROM stdin;
\.


--
-- Name: pin_id_pin_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.pin_id_pin_seq', 18, true);


--
-- Name: temporary_token_id_token_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.temporary_token_id_token_seq', 21, true);


--
-- Name: temporary_uniqid_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.temporary_uniqid_id_seq', 1, true);


--
-- Name: token_id_token_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.token_id_token_seq', 10, true);


--
-- Name: user_cloud_id_user_cloud_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_cloud_id_user_cloud_seq', 7, true);


--
-- Name: user_id_user_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_id_user_seq', 1, false);


--
-- Name: user_validation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_validation_id_seq', 2, true);


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

