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
19	8	308767	2025-02-08 19:40:38	2025-02-08 19:42:08
20	8	338317	2025-02-08 19:48:37	2025-02-08 19:50:07
21	8	476564	2025-02-08 19:53:04	2025-02-08 19:54:34
22	8	88757	2025-02-09 06:29:25	2025-02-09 06:30:55
23	8	214025	2025-02-09 07:30:51	2025-02-09 07:32:21
24	9	750137	2025-02-09 07:46:59	2025-02-09 07:48:29
\.


--
-- Data for Name: temporary_token; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.temporary_token (id_token, token, date_debut, date_fin, id_user) FROM stdin;
22	eRRqRffZgN7cSfGgxpjT9-btRd54DjCqzxUk7Nfc5uk	2025-02-08 19:40:38	2025-02-08 20:40:38	8
23	KVaJUOJGQE2nx6GW3m-sBv1wUSKzyAI_kEweT6WSnXA	2025-02-08 19:48:36	2025-02-08 20:48:36	8
24	Dv95FZflZyv3U5tUKdlXN3cHtio9E4oakL3u_h0Zoj4	2025-02-08 19:53:03	2025-02-08 20:53:03	8
25	juyTAxDwYyUKVvo73CaM1Zr_rgYQST8KIpxVRJbtjZY	2025-02-09 06:29:24	2025-02-09 07:29:24	8
26	n8NvLCKTQ-6ZY8dQHCKyCHjUbbupagnbrnCEE3yee-U	2025-02-09 07:30:49	2025-02-09 08:30:49	8
27	4jztClIHiAPkV0n-FaNNjOe0fvLLiE4tg65CPq5qWwg	2025-02-09 07:46:58	2025-02-09 08:46:58	9
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
11	-_nQ7UoZOnwbwYbUZ8BX6lCPsR5XYgCqAm93eCrJyX8	2025-02-08 19:41:17	2025-02-08 20:41:17	8
12	zHuRejeC98Wgv3HWU39Selr7EzVOS2Ll7PNMTwt4doE	2025-02-08 19:49:04	2025-02-08 20:49:04	8
13	YD47AJtX4SV5rj0TmnDTHeo6B6y0C0hPVU9mGhqA_84	2025-02-08 19:53:42	2025-02-08 20:53:42	8
14	b1a5dbcc-8c5d-4da2-8967-d88ce135272d	2025-02-08 20:49:47	2025-02-15 20:49:47	8
15	eb282a3c-6fd7-4548-81d7-4d117901023c	2025-02-08 19:45:15	2025-02-15 19:45:15	8
16	b1a5dbcc-8c5d-4da2-8967-d88ce135272d	2025-02-08 20:49:47	2025-02-15 20:49:47	8
17	eb282a3c-6fd7-4548-81d7-4d117901023c	2025-02-08 19:45:15	2025-02-15 19:45:15	8
18	6cf25d8c-cc66-4840-8df9-317a1b5165ac	2025-02-08 20:16:33	2025-02-15 20:16:33	8
19	6cf25d8c-cc66-4840-8df9-317a1b5165ac	2025-02-08 20:16:33	2025-02-15 20:16:33	8
21	f1be9466-6b68-4b18-b098-c9d0b4312c6d	2025-02-08 20:16:25	2025-02-15 20:16:25	8
20	f1be9466-6b68-4b18-b098-c9d0b4312c6d	2025-02-08 20:16:25	2025-02-15 20:16:25	8
22	90773f13-3ac8-4a1e-b430-40c7e0cd00c6	2025-02-09 04:49:01	2025-02-16 04:49:01	8
23	90773f13-3ac8-4a1e-b430-40c7e0cd00c6	2025-02-09 04:49:01	2025-02-16 04:49:01	8
25	178639b6-532c-4e17-881a-59d44da6c933	2025-02-08 20:08:23	2025-02-15 20:08:23	8
24	178639b6-532c-4e17-881a-59d44da6c933	2025-02-08 20:08:23	2025-02-15 20:08:23	8
26	e7a051ce-a30a-4dfd-87bb-85d171759539	2025-02-08 20:42:51	2025-02-15 20:42:51	8
27	e7a051ce-a30a-4dfd-87bb-85d171759539	2025-02-08 20:42:51	2025-02-15 20:42:51	8
28	ae4fb20f-d804-4bf1-be40-de167dbfa0c5	2025-02-08 20:36:07	2025-02-15 20:36:07	8
29	ae4fb20f-d804-4bf1-be40-de167dbfa0c5	2025-02-08 20:36:07	2025-02-15 20:36:07	8
30	592dcf46-d3cc-46a1-94cc-fb1377f2c8db	2025-02-08 20:51:06	2025-02-15 20:51:06	8
31	592dcf46-d3cc-46a1-94cc-fb1377f2c8db	2025-02-08 20:51:06	2025-02-15 20:51:06	8
32	4En-miVlzNe23pEQQ58iDZLzhFfUZ2-yuH5RK5y1g70	2025-02-09 06:30:01	2025-02-09 07:30:01	8
33	51f4f1ed-41e2-4ea9-828c-b3b86b8ff4a7	2025-02-09 07:02:36	2025-02-16 07:02:36	8
34	2bcc6db3-b86b-442f-bfb8-a5f332f806ca	2025-02-09 07:02:38	2025-02-16 07:02:38	8
35	3ceac38b-eeae-409c-805a-4889397e4321	2025-02-09 07:02:48	2025-02-16 07:02:48	8
36	13a44123-a097-4869-b114-bc656b4e2851	2025-02-09 07:07:16	2025-02-16 07:07:16	8
37	xtHUp2jfiXhDjtpAETZcvRz929nRXAaj4PmTy4NT1QI	2025-02-09 07:31:25	2025-02-09 08:31:25	8
38	Elo2DcG7XacF8rUOgfafimUZwUk0Cb9MM5bDvh4AY0Y	2025-02-09 07:47:25	2025-02-09 08:47:25	9
39	799e2b51-6ec7-48d7-9e6c-3e358a5b23e6	2025-02-09 09:04:29	2025-02-16 09:04:29	8
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
9	nyavorandrianarisoa.auth@gmail.com	Ny Avo auth	cTJ5RUgQVzchlM+7eBntAb+EwVA=|ws3ll44rpH8tJjJyaRjGf7+VJxCKUN3PmnDQ9uG83Ss=	0	\N
\.


--
-- Data for Name: user_validation; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_validation (id, username, email, password) FROM stdin;
\.


--
-- Name: pin_id_pin_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.pin_id_pin_seq', 24, true);


--
-- Name: temporary_token_id_token_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.temporary_token_id_token_seq', 27, true);


--
-- Name: temporary_uniqid_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.temporary_uniqid_id_seq', 1, true);


--
-- Name: token_id_token_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.token_id_token_seq', 39, true);


--
-- Name: user_cloud_id_user_cloud_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_cloud_id_user_cloud_seq', 9, true);


--
-- Name: user_id_user_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_id_user_seq', 1, false);


--
-- Name: user_validation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_validation_id_seq', 9, true);


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

